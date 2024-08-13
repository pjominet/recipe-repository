using RecipeRepository.Logic.Models.Nomenclature;

namespace RecipeRepository.Logic.Interfaces;

public interface ITagService
{
    public Task<IEnumerable<Tag>> GetTags();
    public Task<Tag?> GetTag(int id);
    public Task<Tag?> CreateTag(Tag tag);
    public Task<bool> UpdateTag(Tag tag);
    public Task<bool> DeleteTag(int id);

    public Task<IEnumerable<TagCategory>> GetTagCategories();
    public Task<TagCategory?> GetTagCategory(int id);
}
