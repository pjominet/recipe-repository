﻿using System.ComponentModel.DataAnnotations;

namespace RecipeRepository.Logic.Models.Identity;

public class RegisterRequest
{
    [Required]
    public string? Username { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MinLength(6)]
    public string? Password { get; set; }

    [Required]
    [Compare("Password")]
    public string? ConfirmPassword { get; set; }

    [Range(typeof(bool), "true", "true")]
    public bool HasAcceptedTerms { get; set; }
}
