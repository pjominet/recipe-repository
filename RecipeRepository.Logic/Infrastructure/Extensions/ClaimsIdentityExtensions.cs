using System.Security.Claims;

namespace RecipeRepository.Logic.Infrastructure.Extensions;

public static class ClaimsIdentityExtensions
{
    /// <summary>
    /// Adds a claim only if the claim type does not yet exist
    /// </summary>
    /// <param name="identity">The <see cref="ClaimsIdentity"/> to try to add the claims to</param>
    /// <param name="type">Type (key) of the claim</param>
    /// <param name="value">Claim value</param>
    /// <returns>True if the claim has been added, false if a claim of the same type existed already</returns>
    public static bool TryAddClaim(this ClaimsIdentity identity, string type, string value)
    {
        if (identity.HasClaim(type, value))
            return false;

        identity.AddClaim(new Claim(type, value));
        return true;
    }
}
