using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RecipeRepository.Data.Entities.Identity;
using RecipeRepository.Logic.Infrastructure.Extensions;
using RecipeRepository.Logic.Infrastructure.Identity;

namespace RecipeRepository.Logic.Infrastructure.Factories;

public class AppUserClaimsFactory(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<AppUser, IdentityRole>(userManager, roleManager, optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        _ = identity.TryAddClaim(CustomIdentityClaims.Name, $"{user.FirstName} {user.LastName}");
        _ = identity.TryAddClaim(CustomIdentityClaims.Email, user.Email!);
        _ = identity.TryAddClaim(CustomIdentityClaims.Guid, $"{user.Id}");

        return identity;
    }
}
