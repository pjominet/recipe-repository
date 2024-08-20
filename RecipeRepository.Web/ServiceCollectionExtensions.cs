using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RecipeRepository.Data.Contexts;
using RecipeRepository.Logic.Infrastructure.Extensions;
using RecipeRepository.Logic.Infrastructure.Factories;
using RecipeRepository.Logic.Infrastructure.Settings;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Services;
using AppUser = RecipeRepository.Data.Entities.Identity.AppUser;

namespace RecipeRepository.Web;

public static class ServiceCollectionExtensions
{
    public static void EnsureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RecipeRepoContext>(options =>
            {
                options.UseSqlServer(configuration.GetDefaultDbConnectionString());
#if DEBUG
                options.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
#endif
            }
        );
        services.AddDatabaseDeveloperPageExceptionFilter();
    }

    public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
        services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
    }

    public static void AddAuthentication(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole>(options =>
            {
#if DEBUG
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 0;
#else
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredUniqueChars = 0;
#endif
            }).AddEntityFrameworkStores<RecipeRepoContext>()
            .AddClaimsPrincipalFactory<AppUserClaimsFactory>()
            .AddDefaultTokenProviders();
    }

    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IFileService, FileService>();

        services.AddScoped<IQuantityService, QuantityService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IUserService, UserService>();
    }
}
