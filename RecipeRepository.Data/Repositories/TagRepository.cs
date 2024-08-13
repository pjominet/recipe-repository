using Microsoft.EntityFrameworkCore;
using RecipeRepository.Data.Entities.Nomenclature;
using RRContext = RecipeRepository.Data.Contexts.RRContext;

namespace RecipeRepository.Data.Repositories;

public class TagRepository(RRContext context) : BaseRepository<RRContext>(context)
{
    public async Task<IList<Tag>> GetTags() => await Context.Tags.ToListAsync();

    public async Task<Tag?> GetTag(int id)
    {
        return await Context.Tags
            .Where(t => t.Id == id)
            .Include(t => t.TagCategory)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteTag(int id)
    {
        try
        {
            await Context.Database.ExecuteSqlInterpolatedAsync($"delete from nomenclature.Tag where Id = {id}");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IList<TagCategory>> GetTagCategories()
    {
        return await Context.TagCategories
            .Include(tc => tc.Tags)
            .ToListAsync();
    }

    public async Task<TagCategory?> GetTagCategory(int id)
    {
        return await Context.TagCategories
            .Include(tc => tc.Tags)
            .FirstOrDefaultAsync();
    }
}
