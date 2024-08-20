using Microsoft.Extensions.Configuration;

namespace RecipeRepository.Logic.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    public static string GetDefaultDbConnectionString(this IConfiguration configuration)
        => configuration.GetConnectionString("DefaultConnection")
           ?? throw new InvalidOperationException("Missing DB connection string!");
}
