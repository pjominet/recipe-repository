using AutoMapper;
using Microsoft.Extensions.Options;
using RecipeRandomizer.Business.Utils.Exceptions;
using RecipeRandomizer.Business.Utils.Settings;
using RecipeRepository.Data.Entities;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Models;
using Recipe = RecipeRepository.Logic.Models.Recipe;
using RRContext = RecipeRepository.Data.Contexts.RRContext;

namespace RecipeRepository.Logic.Services;

public class RecipeService(RRContext context, IMapper mapper, IOptions<AppSettings> appSettings, IFileService fileService)
    : IRecipeService
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

    public async Task<Recipe?> CreateRecipe(Recipe recipe)
    {
        var newRecipe = mapper.Map<Recipe>(recipe);
        newRecipe.CreatedOn = DateTime.UtcNow;
        newRecipe.UpdatedOn = DateTime.UtcNow;

        _recipeRepository.Insert(newRecipe);
        var result = await _recipeRepository.SaveChangesAsync();

        if (!result)
            return null;

        foreach (var tag in recipe.Tags)
            _recipeRepository.Insert(new RecipeTags {TagId = tag.Id, RecipeId = newRecipe.Id});

        result &= await _recipeRepository.SaveChangesAsync();
        return result ? mapper.Map<Recipe>(newRecipe) : null;
    }

    public async Task<bool> UploadRecipeImage(Stream sourceStream, string untrustedFileName, int id)
    {
        var recipe = await _recipeRepository.GetRecipe(id);
        if (recipe is null)
            throw new KeyNotFoundException("Recipe to add image to could not be found");

        try
        {
            var proposedFileExtension = Path.GetExtension(untrustedFileName);
            fileService.CheckForAllowedSignature(sourceStream, proposedFileExtension);

            // delete old recipe image (if any) to avoid file clutter
            var physicalRoot = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot");
            if(!string.IsNullOrWhiteSpace(recipe.ImageUri))
                fileService.DeleteExistingFile(Path.Combine(physicalRoot, recipe.ImageUri));

            // save new recipe image
            var trustedFileName = Guid.NewGuid() + proposedFileExtension;
            await fileService.SaveFileToDiskAsync(sourceStream, Path.Combine(physicalRoot, _appSettings.UserAvatarsFolder), trustedFileName);

            recipe.ImageUri = Path.Combine(_appSettings.RecipeImagesFolder, trustedFileName);
            recipe.OriginalImageName = untrustedFileName;
            recipe.UpdatedOn = DateTime.UtcNow;
            return await _recipeRepository.SaveChangesAsync();
        }
        catch (IOException e)
        {
            Console.WriteLine(e);
            throw new BadRequestException(e.Message);
        }
    }

    public async Task<bool> UpdateRecipe(Recipe recipe)
    {
        var existingRecipe = await _recipeRepository.GetRecipe(recipe.Id, true);
        if (existingRecipe is null)
            return false;

        mapper.Map(recipe, existingRecipe);
        existingRecipe.UpdatedOn = DateTime.UtcNow;
        return await _recipeRepository.SaveChangesAsync();
    }

    public async Task<bool> DeleteRecipe(int id, bool hard = false)
    {
        if (hard)
            _recipeRepository.HardDeleteRecipe(id);
        else
        {
            var recipeToDelete = await _recipeRepository.GetRecipe(id);
            if (recipeToDelete is null)
                return false;

            recipeToDelete.DeletedOn = DateTime.UtcNow;
        }

        return await _recipeRepository.SaveChangesAsync();
    }

    public async Task<Recipe?> RestoreDeletedRecipe(int id)
    {
        var recipeToRestore = await _recipeRepository.GetRecipe(id);
        if (recipeToRestore is null)
            return null;

        recipeToRestore.DeletedOn = null;

        return await _recipeRepository.SaveChangesAsync() ? mapper.Map<Recipe>(recipeToRestore) : null;
    }

    public async Task<bool> ToggleRecipeLike(int recipeId, LikeRequest request)
    {
        if (request.Like)
        {
            _recipeRepository.Insert(new RecipeLikes
            {
                RecipeId = recipeId,
                UserId = request.LikedById!
            });
        }
        else await _recipeRepository.DeleteRecipeLike(recipeId, request.LikedById!);

        return await _recipeRepository.SaveChangesAsync();
    }

    public async Task<bool> AttributeRecipe(AttributionRequest request)
    {
        var recipe = await _recipeRepository.GetRecipe(request.RecipeId);

        if (recipe is null)
            throw new KeyNotFoundException("Recipe does not exist");

        recipe.UserId = request.UserId;
        recipe.UpdatedOn = DateTime.UtcNow;
        return await _recipeRepository.SaveChangesAsync();
    }
}
