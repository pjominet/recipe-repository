using System.ComponentModel.DataAnnotations;

namespace RecipeRepository.Logic.Models;

public class AttributionRequest
{
    [Required]
    public string? UserId { get; set; }

    [Required]
    public int RecipeId { get; set; }
}
