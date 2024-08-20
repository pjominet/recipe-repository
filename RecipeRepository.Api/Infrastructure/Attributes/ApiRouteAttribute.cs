using Microsoft.AspNetCore.Mvc;

namespace RecipeRepository.Api.Infrastructure.Attributes;

public class ApiRouteAttribute : RouteAttribute
{
    private const string BaseRoute = "api";

    public ApiRouteAttribute(string template) : base($"{BaseRoute}/{template}") { }
    public ApiRouteAttribute() : base($"{BaseRoute}") { }
}
