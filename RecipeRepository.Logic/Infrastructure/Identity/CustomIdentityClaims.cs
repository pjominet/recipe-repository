namespace RecipeRepository.Logic.Infrastructure.Identity;

public abstract class CustomIdentityClaims
{
    private const string Prefix = "RR.";

    public const string Name = Prefix + "fullName";
    public const string Email = Prefix + "email";
    public const string Guid = Prefix + "guid";
}
