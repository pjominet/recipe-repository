namespace RecipeRepository.Logic.Models.Nomenclature;

public class QuantityUnit
{
    public int Id { get; set; }
    public required string Label { get; set; }
    public string? Description { get; set; }

    public IList<Ingredient> Ingredients { get; set; }
}
