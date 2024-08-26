using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeRepository.Api.Infrastructure;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Models;

namespace RecipeRepository.Api.Controllers;

[Authorize]
[Route("recipes")]
public class RecipeController(IRecipeService recipeService) : ApiController
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes([FromQuery(Name = "tag")] int[] tagIds)
    {
        return Ok(await recipeService.GetRecipes(tagIds));
    }

    [HttpGet("published/count")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetPublishedRecipeCount()
    {
        return Ok(await recipeService.GetPublishedRecipeCount());
    }

    [HttpGet("created/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipesForUser([FromRoute(Name = "id")] string id)
    {
        return Ok(await recipeService.GetUserRecipes(id));
    }

    [HttpGet("liked/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetLikedRecipesForUser([FromRoute(Name = "id")] string id)
    {
        return Ok(await recipeService.GetUserLikedRecipes(id));
    }

    [HttpGet("deleted")]
    [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetDeletedRecipes()
    {
        return Ok(await recipeService.GetDeletedRecipes());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Recipe), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Recipe>> GetRecipe([FromRoute] int id)
    {
        var recipe = await recipeService.GetRecipe(id);
        return recipe is not null
            ? Ok(recipe)
            : NotFound();
    }

    [HttpGet("random")]
    [ProducesResponseType(typeof(Recipe), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Recipe>> GetRandomRecipe([FromQuery(Name = "tag")] int[] tagIds)
    {
        var recipeId = await recipeService.GetRandomRecipeId(tagIds);
        return recipeId.HasValue
            ? Ok(recipeId.Value)
            : NotFound();
    }

    [HttpGet("abandoned")]
    [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetAbandonedRecipes()
    {
        return Ok(await recipeService.GetAbandonedRecipes());
    }

    [HttpPost("abandoned")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AttributeRecipe([FromBody] AttributionRequest request)
    {
        var result = await recipeService.AttributeRecipe(request);
        return result.Match(
            IActionResult (_) => NoContent(),
            notFound => NotFound(notFound.Message),
            error => Problem(error.Message, HttpContext.TraceIdentifier, StatusCodes.Status500InternalServerError, "Recipe Attribution Error", RfcTypeRef.Ref500)
        );
    }

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Recipe), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Recipe>> AddRecipe([FromBody] Recipe recipe)
    {
        var result = await recipeService.CreateRecipe(recipe);
        return result.Match(
            newRecipe => CreatedAtAction(nameof(GetRecipe), new { id = newRecipe.Id }, newRecipe),
            error => Problem(error.Message, HttpContext.TraceIdentifier, StatusCodes.Status500InternalServerError, "Recipe Creation Error", RfcTypeRef.Ref500)
        );
    }

    [HttpPost("upload/image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadRecipeImage(IFormFile image, [FromQuery(Name = "id")] int? recipeId)
    {
        if (image is not { Length: > 0 } || !recipeId.HasValue)
            return BadRequest("Missing information");

        if (image.Length > 2097152) // > 2MB
            return BadRequest("File is too large");

        var stream = image.OpenReadStream();
        var result = await recipeService.UploadRecipeImage(stream, image.FileName, recipeId.Value);
        stream.Close();

        return result.Match(
            IActionResult (_) => NoContent(),
            notFound => NotFound(notFound.Message),
            error => Problem(error.Message, HttpContext.TraceIdentifier, StatusCodes.Status500InternalServerError, "Image Upload Error", RfcTypeRef.Ref500)
        );
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRecipe([FromRoute] int id, [FromQuery(Name = "hard")] bool hard = false)
    {
        var result = await recipeService.DeleteRecipe(id, hard);
        return result.Match(
            IActionResult (_) => NoContent(),
            notFound => NotFound(notFound.Message),
            error => Problem(error.Message, HttpContext.TraceIdentifier, StatusCodes.Status500InternalServerError, "Image Upload Error", RfcTypeRef.Ref500)
        );
    }

    [HttpGet("restore/{id:int}")]
    [ProducesResponseType(typeof(Recipe), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Recipe>> RestoreDeletedRecipe([FromRoute] int id)
    {
        var result = await recipeService.RestoreDeletedRecipe(id);
        return result.Match(
            Ok,
            notFound => NotFound(notFound.Message),
            error => Problem(error.Message, HttpContext.TraceIdentifier, StatusCodes.Status500InternalServerError, "Recipe Restore Error", RfcTypeRef.Ref500)
        );
    }

    [HttpPost("like/{recipeId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ToggleRecipeLike([FromRoute(Name = "recipeId")] int recipeId, [FromBody] LikeRequest request)
    {
        var result = await recipeService.ToggleRecipeLike(recipeId, request);
        return result.Match(
            IActionResult (_) => NoContent(),
            error => Problem(error.Message, HttpContext.TraceIdentifier, StatusCodes.Status500InternalServerError, "Toggle Like Error", RfcTypeRef.Ref500)
        );
    }

    [HttpPut]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EditRecipe([FromBody] Recipe recipe)
    {
        var result = await recipeService.UpdateRecipe(recipe);
        return result.Match(
            Ok,
            found => NotFound(found.Message),
            error => Problem(error.Message, HttpContext.TraceIdentifier, StatusCodes.Status500InternalServerError, "Edit Recipe Error", RfcTypeRef.Ref500)
        );
    }
}
