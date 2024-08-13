namespace RecipeRepository.Logic.Models.Nomenclature;

public class TagCategory
{
    public int Id { get; set; }
    public required string Label { get; set; }

    public IList<Tag> Tags { get; set; } = [];
}
