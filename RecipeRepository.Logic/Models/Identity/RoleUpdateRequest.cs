using System.ComponentModel.DataAnnotations;

namespace RecipeRepository.Logic.Models.Identity;

public class RoleUpdateRequest
{
    [Required]
    public AppRole? AppRole { get; set; }
}
