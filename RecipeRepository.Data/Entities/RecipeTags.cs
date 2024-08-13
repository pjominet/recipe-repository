using RecipeRepository.Data.Entities.Nomenclature;

namespace RecipeRepository.Data.Entities;

public sealed class RecipeTags
{
    public int RecipeId { get; set; }
    public int TagId { get; set; }

    public Recipe? Recipe { get; set; }
    public Tag? Tag { get; set; }
}
