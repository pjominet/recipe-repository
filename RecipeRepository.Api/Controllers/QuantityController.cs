using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeRepository.Data.Entities.Nomenclature;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Api.Infrastructure.Attributes;

namespace RecipeRepository.Api.Controllers;

[Authorize]
[ApiRoute("quantities")]
public class QuantityController(IQuantityService quantityService) : ApiController
{
    [HttpGet("units")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<QuantityUnit>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<QuantityUnit>>> GetQuantityUnits()
    {
        return Ok(await quantityService.GetQuantityUnits());
    }
}
