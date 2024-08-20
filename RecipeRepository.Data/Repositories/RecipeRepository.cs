using Microsoft.EntityFrameworkCore;
using RecipeRepository.Data.Contexts;
using RecipeRepository.Data.Entities;

namespace RecipeRepository.Data.Repositories;

public class RecipeRepository(RecipeRepoContext context) : BaseRepository<RecipeRepoContext>(context)
{
    public async Task<int> GetPublishedRecipeCount()
    {
        return await Context.Recipes
            .Where(r => r.UserId != null)
            .CountAsync(r => r.DeletedOn == null);
    }

    public async Task<IList<Recipe>> GetRecipes(bool deleted = false)
    {
        return await Context.Recipes
            .Where(r => r.UserId != null)
            .Include(r => r.Cost)
            .Include(r => r.Difficulty)
            .Include(r => r.User)
            .Include(r => r.Ingredients).ThenInclude(i => i.QuantityUnit)
            .Include(r => r.RecipeTags).ThenInclude(rta => rta.Tag)
            .Where(r => r.DeletedOn != null == deleted)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<Recipe?> GetRandomRecipe(IList<int> tagIds)
    {
        var total = await Context.Recipes.CountAsync();
        var rnd = new Random();

        var recipes = Context.Recipes.Select(r => r);
        if (tagIds.Any())
            recipes = GetRecipesFromTagsAsQueryable(tagIds);

        return await recipes
            .Skip(rnd.Next(0, total))
            .FirstOrDefaultAsync();
    }

    public async Task<IList<Recipe>> GetRecipesFromTags(IList<int> tagIds)
    {
        return await GetRecipesFromTagsAsQueryable(tagIds).AsSplitQuery().ToListAsync();
    }

    public async Task<IList<Recipe>> GetUserLikedRecipes(string userId)
    {
        var recipes = Context.RecipeLikes
            .Where(rl => rl.UserId == userId)
            .Select(rl => rl.Recipe)
            .Where(r => r.DeletedOn == null)
            .Distinct();

        if (!recipes.Any())
            return [];

        return await recipes
            .Include(r => r.User)
            .Include(r => r.Ingredients).ThenInclude(i => i.QuantityUnit)
            .Include(r => r.RecipeTags).ThenInclude(rta => rta.Tag)
            .AsSplitQuery()
            .ToListAsync();
    }

    public void HardDeleteRecipe(int id)
    {
        /*foreach (var tagAssociation in Context.RecipeTagAssociations.Where(rta => rta.RecipeId == id))
            Delete(tagAssociation);*/

        Parallel.ForEach(Context.RecipeTagAssociations.Where(rta => rta.RecipeId == id), Delete);

        /*foreach (var recipeLike in Context.RecipeLikes.Where(rl => rl.RecipeId == id))
            Delete(recipeLike);*/

        Parallel.ForEach(Context.RecipeLikes.Where(rl => rl.RecipeId == id), Delete);

        Delete(Context.Recipes.Where(r => r.Id == id));
    }

    public async Task<IList<Recipe>> GetAbandonedRecipes()
    {
        return await Context.Recipes
            .Include(r => r.RecipeLikes)
            .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
            .Where(r => r.UserId != null)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<Recipe?> GetRecipe(int id, bool includeDetail = false)
    {
        var query = Context.Recipes
            .Where(r => r.Id == id);

        if (includeDetail)
            query = query.Include(r => r.Cost)
                .Include(r => r.Difficulty)
                .Include(r => r.User)
                .Include(r => r.Ingredients).ThenInclude(i => i.QuantityUnit)
                .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
                .Include(r => r.RecipeLikes);

        return await query
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }

    public async Task<IList<Recipe>> GetUserRecipes(string userId)
    {
        return await Context.Recipes
            .Include(r => r.Cost)
            .Include(r => r.Difficulty)
            .Include(r => r.Ingredients).ThenInclude(i => i.QuantityUnit)
            .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> DeleteRecipeLike(int recipeId, string likedById)
    {
        try
        {
            await Context.Database.ExecuteSqlInterpolatedAsync($"delete from rr.RecipeLikes where RecipeId = {recipeId} and UserId = {likedById}");
            return true;
        }
        catch
        {
            return false;
        }
    }

    #region helpers

    private IQueryable<Recipe> GetRecipesFromTagsAsQueryable(ICollection<int> tagIds)
    {
        var matchingRecipeIds = Context.RecipeTagAssociations
            .Where(rta => tagIds.Contains(rta.TagId))
            .GroupBy(rta => rta.RecipeId)
            .Where(grp => grp.Count() == tagIds.Count)
            .Select(grp => grp.Key);

        var recipes = Context.Recipes
            .Where(r => matchingRecipeIds.Contains(r.Id))
            .Where(r => r.DeletedOn == null)
            .Where(r => r.UserId != null)
            .OrderBy(r => r.CreatedOn);

        if (!recipes.Any())
            return Enumerable.Empty<Recipe>().AsQueryable();

        return recipes
            .Include(r => r.User)
            .Include(r => r.Ingredients).ThenInclude(i => i.QuantityUnit)
            .Include(r => r.RecipeTags).ThenInclude(rta => rta.Tag);
    }

    #endregion

}
