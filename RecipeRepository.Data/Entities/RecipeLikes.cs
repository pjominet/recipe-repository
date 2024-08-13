using RecipeRepository.Data.Entities.Identity;

namespace RecipeRepository.Data.Entities;

public class RecipeLikes
{
    public int RecipeId { get; set; }
    public required string UserId { get; set; }

    public Recipe? Recipe { get; set; }
    public AppUser? User { get; set; }
}
