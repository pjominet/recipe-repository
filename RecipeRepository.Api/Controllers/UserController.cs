using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeRepository.Api.Infrastructure;
using RecipeRepository.Api.Infrastructure.Extensions;
using RecipeRepository.Logic.Infrastructure.Extensions;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Models.Identity;

namespace RecipeRepository.Api.Controllers;

[Route("users")]
[Authorize]
public class UserController(IUserService userService) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AppUser>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        return Ok(await userService.GetUsers());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AppUser), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppUser>> GetUser([FromRoute] string id)
    {
        // users can get their own account and admins can get any account
        if (id != User.GetId())
            return Forbid("This action is forbidden!");

        var user = await userService.GetUser(id);
        return user is null
            ? NotFound()
            : Ok(user);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AppUser), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppUser>> UpdateUser([FromRoute] string id, [FromBody] UserUpdateRequest userUpdateRequest)
    {
        // users can update their own account and admins can update any account
        if (id != User.GetId())
            return Forbid("User is not allowed to delete it's own account!");

        var result = await userService.UpdateUser(id, userUpdateRequest);
        return result.Match(
            Ok,
            badRequest => BadRequest(badRequest.Message),
            notFound => NotFound(notFound.Message),
            error => Problem(error.Message, HttpContext.TraceIdentifier, StatusCodes.Status500InternalServerError, "Delete User Error", RfcTypeRef.Ref500)
            );
    }

    [HttpPost("image-upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadUserAvatar([FromForm(Name = "file")] IFormFile image, [FromForm(Name = "id")] string? userId)
    {
        if (image is not { Length: > 0 } || !userId.HasValue())
            return BadRequest("Missing information");

        if (image.Length > 2097152) // > 2MB
            return BadRequest("File is too large (> 2MB)");

        var stream = image.OpenReadStream();
        var result = await userService.UploadUserAvatar(stream, image.FileName, userId!);
        stream.Close();

        return result.Match(
            IActionResult (_) => NoContent(),
            notFound => NotFound(notFound.Message),
            error => Problem(error.Message, HttpContext.TraceIdentifier, StatusCodes.Status500InternalServerError, "Upload Image Error", RfcTypeRef.Ref500)
        );
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser([FromRoute] string id)
    {
        // users can delete their own account and admins can delete any account
        if (id != User.GetId())
            return Forbid("This action is forbidden!");

        var result = await userService.DeleteUser(id);
        return result.Match(
            IActionResult (_) => NoContent(),
            notFound => NotFound(notFound.Message),
            error => Problem(error.Message, HttpContext.TraceIdentifier, StatusCodes.Status500InternalServerError, "Delete User Error", RfcTypeRef.Ref500)
        );
    }
}
