using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeRepository.Logic.Infrastructure.Extensions;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Models;
using RecipeRepository.Logic.Models.Identity;

namespace RecipeRepository.Api.Controllers;

[Route("users")]
[Authorize]
public class UserController(IUserService userService) : ApiController
{
    // returns the current authenticated user (null if not no valid jwt token was received)
    private new AppUser? User => (AppUser?)HttpContext.Items[$"{nameof(AppUser)}"];

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AppUser>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        return Ok(await userService.GetUsers());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(AppUser), StatusCodes.Status200OK)]
    public async Task<ActionResult<AppUser>> GetUser([FromRoute] string id)
    {
        // users can get their own account and admins can get any account
        if (new Guid(id) != User?.Id)
            return Unauthorized(new SimpleResponse { Message = "Unauthorized" });

        return Ok(await userService.GetUser(id));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(AppUser), StatusCodes.Status200OK)]
    public async Task<ActionResult<AppUser>> UpdateUser([FromRoute] string id, [FromBody] UserUpdateRequest userUpdateRequest)
    {
        // users can update their own account and admins can update any account
        if (new Guid(id) != User?.Id)
            return Unauthorized(new SimpleResponse { Message = "Unauthorized" });

        return Ok(await userService.Update(id, userUpdateRequest));
    }

    [HttpPost("image-upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadUserAvatar([FromForm(Name = "file")] IFormFile image, [FromForm(Name = "id")] string? userId)
    {
        if (image is not { Length: > 0 } || !userId.HasValue())
            return BadRequest("Missing information");

        if (image.Length > 2097152) // > 2MB
            return BadRequest("File is too large (> 2MB)");

        var stream = image.OpenReadStream();
        var result = await userService.UploadUserAvatar(stream, image.FileName, userId!);
        stream.Close();

        return result
            ? NoContent()
            : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser([FromRoute] string id)
    {
        // users can delete their own account and admins can delete any account
        if (new Guid(id) != User?.Id)
            return Unauthorized(new { message = "Unauthorized" });

        return await userService.Delete(id)
            ? Ok(new SimpleResponse { Message = "Account deleted successfully" })
            : StatusCode(StatusCodes.Status500InternalServerError);
    }
}
