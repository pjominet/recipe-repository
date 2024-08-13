using System.ComponentModel.DataAnnotations;

namespace RecipeRepository.Logic.Models.Identity;

public class ChangePasswordRequest
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string? Password { get; set; }

    [Required]
    [MinLength(6)]
    public string? NewPassword { get; set; }

    [Required]
    [Compare("NewPassword")]
    public string? ConfirmNewPassword { get; set; }
}
