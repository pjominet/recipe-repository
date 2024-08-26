using AutoMapper;
using OneOf;
using Microsoft.Extensions.Options;
using RecipeRepository.Data.Contexts;
using RecipeRepository.Logic.Infrastructure.Extensions;
using RecipeRepository.Logic.Infrastructure.OneOfResults;
using RecipeRepository.Logic.Infrastructure.Settings;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Models;
using Entities = RecipeRepository.Data.Entities;

namespace RecipeRepository.Logic.Services;

public class RecipeService(RecipeRepoContext context, IMapper mapper, IOptions<AppSettings> appSettings, IFileService fileService) : IRecipeService
{
    private readonly Data.Repositories.RecipeRepository _recipeRepository = new(context);
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<IEnumerable<Recipe>> GetRecipes(IList<int> tagIds)
    {
        return mapper.Map<IEnumerable<Recipe>>(tagIds.Any()
            ? await _recipeRepository.GetRecipesFromTags(tagIds)
            : await _recipeRepository.GetRecipes());
    }

    public async Task<int> GetPublishedRecipeCount()
        => await _recipeRepository.GetPublishedRecipeCount();

    public async Task<IEnumerable<Recipe>> GetDeletedRecipes()
        => mapper.Map<IEnumerable<Recipe>>(await _recipeRepository.GetRecipes(true));

    public async Task<IEnumerable<Recipe>> GetAbandonedRecipes()
        => mapper.Map<IEnumerable<Recipe>>(await _recipeRepository.GetAbandonedRecipes());

    public async Task<Recipe?> GetRecipe(int id)
        => mapper.Map<Recipe?>(await _recipeRepository.GetRecipe(id, true));

    public async Task<int?> GetRandomRecipeId(IList<int> tagIds)
        => (await _recipeRepository.GetRandomRecipe(tagIds))?.Id;

    public async Task<IEnumerable<Recipe>> GetUserRecipes(string userId)
        => mapper.Map<IEnumerable<Recipe>>(await _recipeRepository.GetUserRecipes(userId));

    public async Task<IEnumerable<Recipe>> GetUserLikedRecipes(string userId)
        => mapper.Map<IEnumerable<Recipe>>(await _recipeRepository.GetUserLikedRecipes(userId));

    public async Task<OneOf<Recipe, Error>> CreateRecipe(Recipe recipe)
    {
        var newRecipe = new Entities.Recipe
        {
            UserId = recipe.UserId,
            Name = recipe.Name,
            Description = recipe.Description,
            NumberOfPeople = recipe.NumberOfPeople,
            CostId = (int)recipe.Cost,
            DifficultyId = (int)recipe.Difficulty,
            PrepTime = recipe.PrepTime,
            CookTime = recipe.CookTime,
            Preparation = recipe.Preparation,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        _recipeRepository.Insert(newRecipe);

        if (await _recipeRepository.SaveChangesAsync())
            return new Error("Recipe could not be persisted!");

        foreach (var tag in recipe.Tags)
            _recipeRepository.Insert(new Entities.RecipeTags { TagId = tag.Id, RecipeId = newRecipe.Id });

        if (await _recipeRepository.SaveChangesAsync())
            return new Error("Recipe tags could not be persisted!");

        recipe.Id = newRecipe.Id;
        return recipe;
    }

    public async Task<OneOf<Success, NotFound, Error>> UploadRecipeImage(Stream sourceStream, string untrustedFileName, int id)
    {
        var recipe = await _recipeRepository.GetRecipe(id);
        if (recipe is null)
            return new NotFound("Recipe does not exist!");

        try
        {
            var proposedFileExtension = Path.GetExtension(untrustedFileName);
            fileService.CheckForAllowedSignature(sourceStream, proposedFileExtension);

            // delete old recipe image (if any) to avoid file clutter
            var physicalRoot = Path.Combine(Directory.GetCurrentDirectory(), _appSettings.RecipeImagesFolder);
            if (recipe.ImageUri.HasValue())
                fileService.DeleteExistingFile(Path.Combine(physicalRoot, recipe.ImageUri!));

            // save new recipe image
            var trustedFileName = Guid.NewGuid() + proposedFileExtension;
            await fileService.SaveFileToDisk(sourceStream, Path.Combine(physicalRoot, _appSettings.RecipeImagesFolder), trustedFileName);

            recipe.ImageUri = Path.Combine(_appSettings.RecipeImagesFolder, trustedFileName);
            recipe.OriginalImageName = untrustedFileName;
            recipe.UpdatedOn = DateTime.UtcNow;

            return !await _recipeRepository.SaveChangesAsync()
                ? new Error("Recipe changes could not be persisted!")
                : new Success();
        }
        catch (IOException e)
        {
            return new Error(e.Message);
        }
    }

    public async Task<OneOf<Success, NotFound, Error>> UpdateRecipe(Recipe recipe)
    {
        var existingRecipe = await _recipeRepository.GetRecipe(recipe.Id, true);
        if (existingRecipe is null)
            return new NotFound("Requested recipe does not exist!");

        mapper.Map(recipe, existingRecipe);
        existingRecipe.UpdatedOn = DateTime.UtcNow;

        return !await _recipeRepository.SaveChangesAsync()
            ? new Error("Recipe changes could not be persisted!")
            : new Success();
    }

    public async Task<OneOf<Success, NotFound, Error>> DeleteRecipe(int id, bool hard = false)
    {
        if (hard)
            _recipeRepository.HardDeleteRecipe(id);
        else
        {
            var recipeToDelete = await _recipeRepository.GetRecipe(id);
            if (recipeToDelete is null)
                return new NotFound("Requested recipe does not exist!");

            recipeToDelete.DeletedOn = DateTime.UtcNow;
        }

        return !await _recipeRepository.SaveChangesAsync()
            ? new Error("Recipe changes could not be persisted!")
            : new Success();
    }

    public async Task<OneOf<Recipe, NotFound, Error>> RestoreDeletedRecipe(int id)
    {
        var recipeToRestore = await _recipeRepository.GetRecipe(id);
        if (recipeToRestore is null)
            return new NotFound("Requested recipe does not exist!");

        recipeToRestore.DeletedOn = null;

        return !await _recipeRepository.SaveChangesAsync()
            ? new Error("Recipe changes could not be persisted!")
            : mapper.Map<Recipe>(recipeToRestore);
    }

    public async Task<OneOf<Success, Error>> ToggleRecipeLike(int recipeId, LikeRequest request)
    {
        if (request.Like)
        {
            _recipeRepository.Insert(new Entities.RecipeLikes
            {
                RecipeId = recipeId,
                UserId = request.LikedById!
            });
        }
        else await _recipeRepository.DeleteRecipeLike(recipeId, request.LikedById!);

        return !await _recipeRepository.SaveChangesAsync()
            ? new Error("Recipe like could not be persisted!")
            : new Success();
    }

    public async Task<OneOf<Success, NotFound, Error>> AttributeRecipe(AttributionRequest request)
    {
        var recipe = await _recipeRepository.GetRecipe(request.RecipeId);

        if (recipe is null)
            return new NotFound("Requested recipe does not exist!");

        recipe.UserId = request.UserId;
        recipe.UpdatedOn = DateTime.UtcNow;
        return !await _recipeRepository.SaveChangesAsync()
            ? new Error("Recipe attribution could not be persisted!")
            : new Success();
    }
}
