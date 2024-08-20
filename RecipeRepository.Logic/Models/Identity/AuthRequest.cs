using System.ComponentModel.DataAnnotations;

namespace RecipeRepository.Logic.Models.Identity;

public class AuthRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
