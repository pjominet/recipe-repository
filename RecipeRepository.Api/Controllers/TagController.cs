using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Models.Nomenclature;
using RecipeRepository.Api.Infrastructure.Attributes;

namespace RecipeRepository.Api.Controllers;

[Authorize]
[ApiRoute("tags")]
public class TagController(ITagService tagService) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Tag>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
    {
        return Ok(await tagService.GetTags());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Tag), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Task>> GetTag([FromRoute] int id)
    {
        var tag = await tagService.GetTag(id);
        return tag is not null
            ? Ok(tag)
            : NotFound();
    }

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Tag), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Tag>> AddTag([FromBody] Tag tag)
    {
        var newTag = await tagService.CreateTag(tag);
        return newTag is not null
            ? CreatedAtAction(nameof(GetTag), new { id = newTag.Id }, newTag)
            : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTag([FromRoute] int id)
    {
        return await tagService.DeleteTag(id)
            ? NoContent()
            : StatusCode(StatusCodes.Status404NotFound);
    }

    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EditTag([FromRoute] int id, [FromBody] Tag tag)
    {
        if (id != tag.Id)
            return BadRequest();

        return await tagService.UpdateTag(tag)
            ? NoContent()
            : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(IEnumerable<TagCategory>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TagCategory>>> GetTagCategories()
    {
        return Ok(await tagService.GetTagCategories());
    }

    [HttpGet("categories/{id:int}")]
    [ProducesResponseType(typeof(TagCategory), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TagCategory>> GetTagCategory([FromRoute] int id)
    {
        var tagCategory = await tagService.GetTagCategory(id);
        return tagCategory is not null
            ? Ok(tagCategory)
            : NotFound();
    }
}
