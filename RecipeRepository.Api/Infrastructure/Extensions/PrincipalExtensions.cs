using System.Security.Claims;
using System.Security.Principal;
using RecipeRepository.Logic.Infrastructure.Identity;

namespace RecipeRepository.Api.Infrastructure.Extensions;

public static class PrincipalExtensions
{
    public static bool IsInAnyRoles(this IPrincipal @this, params string[] roles) => roles.Any(@this.IsInRole);

    public static string GetId(this IPrincipal currentPrincipal) => currentPrincipal.GetClaimValue(CustomIdentityClaims.Guid);
    public static string GetName(this IPrincipal currentPrincipal) => currentPrincipal.GetClaimValue(CustomIdentityClaims.Name);
    public static string GetEmail(this IPrincipal currentPrincipal) => currentPrincipal.GetClaimValue(CustomIdentityClaims.Email);

    private static string GetClaimValue(this IPrincipal currentPrincipal, string key)
    {
        return currentPrincipal.Identity is ClaimsIdentity identity
            ? identity.Claims.Where(c => c.Type == key).Select(c => c.Value).FirstOrDefault() ?? string.Empty
            : string.Empty;

    }
}
