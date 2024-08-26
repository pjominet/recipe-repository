using OneOf;
using RecipeRepository.Logic.Infrastructure.OneOfResults;
using RecipeRepository.Logic.Models;

namespace RecipeRepository.Logic.Interfaces;

public interface IRecipeService
{
    public Task<IEnumerable<Recipe>> GetRecipes(IList<int> tagIds);
    public Task<int> GetPublishedRecipeCount();
    public Task<IEnumerable<Recipe>> GetDeletedRecipes();
    public Task<IEnumerable<Recipe>> GetAbandonedRecipes();
    public Task<Recipe?> GetRecipe(int id);
    public Task<int?> GetRandomRecipeId(IList<int> tagIds);
    public Task<IEnumerable<Recipe>> GetUserRecipes(string userId);
    public Task<IEnumerable<Recipe>> GetUserLikedRecipes(string userId);
    public Task<OneOf<Recipe, Error>> CreateRecipe(Recipe recipe);
    public Task<OneOf<Success, NotFound, Error>> UploadRecipeImage(Stream sourceStream, string untrustedFileName, int id);
    public Task<OneOf<Success, NotFound, Error>> UpdateRecipe(Recipe recipe);
    public Task<OneOf<Success, NotFound, Error>> DeleteRecipe(int id, bool hard = false);
    public Task<OneOf<Recipe, NotFound, Error>> RestoreDeletedRecipe(int id);
    public Task<OneOf<Success, Error>> ToggleRecipeLike(int recipeId, LikeRequest request);
    public Task<OneOf<Success, NotFound, Error>> AttributeRecipe(AttributionRequest request);
}
