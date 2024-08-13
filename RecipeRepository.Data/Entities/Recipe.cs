using RecipeRepository.Data.Entities.Identity;
using RecipeRepository.Data.Entities.Nomenclature;

namespace RecipeRepository.Data.Entities;

public class Recipe
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUri { get; set; }
    public string? OriginalImageName { get; set; }
    public int NumberOfPeople { get; set; }
    public int CostId { get; set; }
    public int DifficultyId { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public required string Preparation { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public DateTime? DeletedOn { get; set; }

    public Cost? Cost { get; set; }
    public Difficulty? Difficulty { get; set; }
    public AppUser? User { get; set; }
    public ICollection<Ingredient> Ingredients { get; set; } = new HashSet<Ingredient>();
    public ICollection<RecipeTags> RecipeTags { get; set; } = new HashSet<RecipeTags>();
    public ICollection<RecipeLikes> RecipeLikes { get; set; } = new HashSet<RecipeLikes>();
}
