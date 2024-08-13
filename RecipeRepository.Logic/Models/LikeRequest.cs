using System.ComponentModel.DataAnnotations;

namespace RecipeRepository.Logic.Models;

public class LikeRequest
{
    [Required]
    public bool Like { get; set; }

    [Required]
    public string? LikedById { get; set; }
}
