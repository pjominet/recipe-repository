using RecipeRepository.Logic;

namespace RecipeRepository.Web;

public class Startup(IConfiguration configuration, ILogger logger)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.EnsureDatabase(configuration);

        services.AddSettings(configuration);
        services.AddAuthentication();
        services.AddAppServices();

        // configure default CORS
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder =>
                policyBuilder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        services.AddAutoMapper(typeof(LogicAssemblyMarker).Assembly);

        services.AddRazorPages().AddRazorRuntimeCompilation();

        services.AddAuthorization();
    }

    public static void Configure(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseCors();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
    }
}
