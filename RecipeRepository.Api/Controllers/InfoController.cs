using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RecipeRepository.Logic.Infrastructure.Settings;
using RecipeRepository.Api.Infrastructure.Attributes;

namespace RecipeRepository.Api.Controllers;

[ApiRoute]
[AllowAnonymous]
public class InfoController(IOptions<AppSettings> appOptions) : ApiController
{
    private readonly AppSettings _appSettings = appOptions.Value;

    [HttpGet]
    [HttpGet("info")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult GetApiInfo()
    {
        return Ok("Recipe Repository API is currently running @" + _appSettings.Version);
    }

    [HttpGet("version")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult GetApiVersion()
    {
        return Ok(_appSettings.Version);
    }

    [HttpGet("love")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult GetLove()
    {
        return Ok("Developed with ❤️ by Pat00");
    }
}
