namespace RecipeRepository.Logic.Models.Identity;

public class AppUser
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public string? ProfileImageUri { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public DateTime? DeletedOn { get; set; }
    public bool IsVerified { get; set; }

    public IList<string> Roles { get; set; } = [];
    public IList<Recipe> Recipes { get; set; } = [];
    public IList<Recipe> LikedRecipes { get; set; } = [];
}
