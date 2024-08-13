using System.ComponentModel.DataAnnotations;

namespace RecipeRepository.Logic.Models.Identity;

public class VerificationRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}
