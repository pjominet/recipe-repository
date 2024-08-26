using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Models;

namespace RecipeRepository.Api.Controllers;

[Route("recipes")]
public class RecipeController(IRecipeService recipeService) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes([FromQuery(Name = "tag")] int[] tagIds)
    {
        return Ok(await recipeService.GetRecipes(tagIds));
    }

    [HttpGet("published-count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetPublishedRecipeCount()
    {
        return Ok(await recipeService.GetPublishedRecipeCount());
    }

    [Authorize]
    [HttpGet("created/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipesForUser([FromRoute(Name = "id")] string id)
    {
        return Ok(await recipeService.GetUserRecipes(id));
    }

    [Authorize]
    [HttpGet("liked/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetLikedRecipesForUser([FromRoute(Name = "id")] string id)
    {
        return Ok(await recipeService.GetUserLikedRecipes(id));
    }

    [Authorize]
    [HttpGet("deleted")]
    [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetDeletedRecipes()
    {
        return Ok(await recipeService.GetDeletedRecipes());
    }

    [Authorize]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Recipe), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Recipe>> GetRecipe([FromRoute] int id)
    {
        var recipe = await recipeService.GetRecipe(id);
        return recipe is not null
            ? Ok(recipe)
            : NotFound();
    }

    [Authorize]
    [HttpGet("random")]
    [ProducesResponseType(typeof(Recipe), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Recipe>> GetRandomRecipe([FromQuery(Name = "tag")] int[] tagIds)
    {
        var recipeId = await recipeService.GetRandomRecipeId(tagIds);
        return recipeId.HasValue
            ? Ok(recipeId.Value)
            : NotFound();
    }

    [Authorize]
    [HttpGet("abandoned")]
    [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetAbandonedRecipes()
    {
        return Ok(await recipeService.GetAbandonedRecipes());
    }

    [Authorize]
    [HttpPost("abandoned")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Recipe>>> AttributeRecipe([FromBody] AttributionRequest request)
    {
        return await recipeService.AttributeRecipe(request)
            ? NoContent()
            : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [Authorize]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Recipe), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Recipe>> AddRecipe([FromBody] Recipe recipe)
    {
        var newRecipe = await recipeService.CreateRecipe(recipe);
        return newRecipe is not null
            ? CreatedAtAction(nameof(GetRecipe), new { id = newRecipe.Id }, newRecipe)
            : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [Authorize]
    [HttpPost("image-upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadRecipeImage([FromForm(Name = "file")] IFormFile image, [FromForm(Name = "id")] int? recipeId)
    {
        if (image is not { Length: > 0 } || !recipeId.HasValue)
            return BadRequest("Missing information");

        if (image.Length > 2097152) // > 2MB
            return BadRequest("File is too large");

        var stream = image.OpenReadStream();
        var result = await recipeService.UploadRecipeImage(stream, image.FileName, recipeId.Value);
        stream.Close();

        return result
            ? NoContent()
            : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRecipe([FromRoute] int id, [FromQuery(Name = "hard")] bool hard = false)
    {
        return await recipeService.DeleteRecipe(id, hard)
            ? NoContent()
            : NotFound();
    }

    [Authorize]
    [HttpGet("restore/{id:int}")]
    [ProducesResponseType(typeof(Recipe), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Recipe>> RestoreDeletedRecipe([FromRoute] int id)
    {
        var restoredRecipe = await recipeService.RestoreDeletedRecipe(id);

        return restoredRecipe is not null
            ? Ok(restoredRecipe)
            : NotFound();
    }

    [Authorize]
    [HttpPost("like/{recipeId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ToggleRecipeLike([FromRoute(Name = "recipeId")] int recipeId, [FromBody] LikeRequest request)
    {
        return await recipeService.ToggleRecipeLike(recipeId, request)
            ? NoContent()
            : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [Authorize]
    [HttpPut]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EditRecipe([FromBody] Recipe recipe)
    {
        return await recipeService.UpdateRecipe(recipe)
            ? Ok(recipe.Id)
            : StatusCode(StatusCodes.Status500InternalServerError);
    }
}
