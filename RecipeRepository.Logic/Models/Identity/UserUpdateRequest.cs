using System.ComponentModel.DataAnnotations;

namespace RecipeRepository.Logic.Models.Identity
{
    public class UserUpdateRequest
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
