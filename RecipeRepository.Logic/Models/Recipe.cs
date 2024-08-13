using RecipeRepository.Logic.Models.Nomenclature;

namespace RecipeRepository.Logic.Models;

public class Recipe
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUri { get; set; }
    public string? OriginalImageName { get; set; }
    public int NumberOfPeople { get; set; }
    public Cost Cost { get; set; }
    public Difficulty Difficulty { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public required string Preparation { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public DateTime? DeletedOn { get; set; }

    public string? CreatedBy { get; set; }
    public IList<Ingredient> Ingredients { get; set; } = [];
    public IList<Tag> Tags { get; set; } = [];
    public IList<int> Likes { get; set; } = [];
}
