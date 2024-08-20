namespace RecipeRepository.Logic.Infrastructure.Settings;

public class AppSettings
{
    public required string Version { get; set; }
    public required string RecipeImagesFolder { get; set; }
    public required string UserAvatarsFolder { get; set; }
}
