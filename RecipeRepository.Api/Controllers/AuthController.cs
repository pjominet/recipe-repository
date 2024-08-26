using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Models.Identity;
using RecipeRepository.Api.Infrastructure.Attributes;

namespace RecipeRepository.Api.Controllers;

[Authorize]
[ApiRoute("auth")]
public class AuthController(IAuthService authService) : ApiController
{

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        if (!await authService.CheckCredentials(request.Email, request.Password))
            return Unauthorized();

        var token = authService.GenerateToken(request.Email);
        return Ok(new { token });
    }
}
