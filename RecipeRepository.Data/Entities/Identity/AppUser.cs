using Microsoft.AspNetCore.Identity;

namespace RecipeRepository.Data.Entities.Identity;

public class AppUser : IdentityUser
{
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public DateTime? DeletedOn { get; set; }

    public ICollection<Recipe> Recipes { get; set; } = new HashSet<Recipe>();
    public ICollection<RecipeLikes> RecipeLikes { get; set; } = new HashSet<RecipeLikes>();
}
