using Microsoft.AspNetCore.Mvc;
using RecipeRepository.Data.Entities.Nomenclature;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Api.Infrastructure.Attributes;

namespace RecipeRepository.Api.Controllers;

[ApiRoute("quantities")]
public class QuantityController(IQuantityService quantityService) : ApiController
{
    [HttpGet("units")]
    [ProducesResponseType(typeof(IEnumerable<QuantityUnit>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<QuantityUnit>>> GetQuantityUnits()
    {
        return Ok(await quantityService.GetQuantityUnitsAsync());
    }
}
