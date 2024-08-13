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
    public Task<Recipe?> CreateRecipe(Recipe recipe);
    public Task<bool> UploadRecipeImage(Stream sourceStream, string untrustedFileName, int id);
    public Task<bool> UpdateRecipe(Recipe recipe);
    public Task<bool> DeleteRecipe(int id, bool hard = false);
    public Task<Recipe?> RestoreDeletedRecipe(int id);
    public Task<bool> ToggleRecipeLike(int recipeId, LikeRequest request);
    public Task<bool> AttributeRecipe(AttributionRequest request);
}
