namespace RecipeRepository.Logic.Models.Nomenclature;

public class Tag
{
    public int Id { get; set; }
    public int TagCategoryId { get; set; }
    public required string Label { get; set; }

    public TagCategory? TagCategory { get; set; }
    public IList<Recipe> Recipes { get; set; } = [];
}
