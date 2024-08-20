using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using RecipeRepository.Data.Contexts;
using RecipeRepository.Logic.Infrastructure.Extensions;
using RecipeRepository.Logic.Infrastructure.Settings;
using RecipeRepository.Logic.Interfaces;
using RecipeRepository.Logic.Services;
using AppUser = RecipeRepository.Data.Entities.Identity.AppUser;

namespace RecipeRepository.Api;

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
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
    }

    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<RecipeRepoContext>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection("JwtSettings:Issuer").Value,
                    ValidAudience = configuration.GetSection("JwtSettings:Audience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JwtSettings:Secret").Value!))
                };
            });
    }

    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IFileService, FileService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IQuantityService, QuantityService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IUserService, UserService>();
    }
}
