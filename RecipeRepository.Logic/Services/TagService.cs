using AutoMapper;
using RecipeRepository.Data.Repositories;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Models.Nomenclature;
using RRContext = RecipeRepository.Data.Contexts.RRContext;
using Entities = RecipeRepository.Data.Entities.Nomenclature;

namespace RecipeRepository.Logic.Services;

public class TagService(RRContext context, IMapper mapper) : ITagService
{
    private readonly TagRepository _tagRepository = new(context);

    public async Task<IEnumerable<Tag>> GetTags() => mapper.Map<IEnumerable<Tag>>(await _tagRepository.GetTags());

    public async Task<Tag?> GetTag(int id) => mapper.Map<Tag?>(await _tagRepository.GetTag(id));

    public async Task<Tag?> CreateTag(Tag tag)
    {
        var newTag = mapper.Map<Entities.Tag>(tag);
        if (newTag is null)
            return null;

        _tagRepository.Insert(newTag);
        return await _tagRepository.SaveChangesAsync()
            ? mapper.Map<Tag>(newTag)
            : null;
    }

    public async Task<bool> UpdateTag(Tag tag)
    {
        var existingRecipe = await _tagRepository.GetTag(tag.Id);
        mapper.Map(tag, existingRecipe);
        return await _tagRepository.SaveChangesAsync();
    }

    public async Task<bool> DeleteTag(int id) => await _tagRepository.DeleteTag(id);

    public async Task<IEnumerable<TagCategory>> GetTagCategories() => mapper.Map<IEnumerable<TagCategory>>(await _tagRepository.GetTagCategories());

    public async Task<TagCategory?> GetTagCategory(int id) => mapper.Map<TagCategory>(await _tagRepository.GetTagCategory(id));
}
