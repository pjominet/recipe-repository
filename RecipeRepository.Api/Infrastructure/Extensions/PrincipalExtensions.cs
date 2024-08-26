using System.Security.Claims;
using System.Security.Principal;
using RecipeRepository.Logic.Infrastructure.Identity;
using RecipeRepository.Logic.Models.Identity;

namespace RecipeRepository.Api.Infrastructure.Extensions;

public static class PrincipalExtensions
{
    public static bool IsInAnyRoles(this IPrincipal @this, params string[] roles) => roles.Any(@this.IsInRole);
    public static bool IsAdmin(this IPrincipal @this) => @this.IsInRole(Roles.Admin);

    public static string GetId(this IPrincipal currentPrincipal) => currentPrincipal.GetClaimValue(CustomIdentityClaims.Guid);
    public static string GetEmail(this IPrincipal currentPrincipal) => currentPrincipal.GetClaimValue(CustomIdentityClaims.Email);

    private static string GetClaimValue(this IPrincipal currentPrincipal, string key)
    {
        return currentPrincipal.Identity is ClaimsIdentity identity
            ? identity.Claims.Where(c => c.Type == key).Select(c => c.Value).FirstOrDefault() ?? string.Empty
            : string.Empty;
    }
}
