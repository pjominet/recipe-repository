using RecipeRepository.Logic;

namespace RecipeRepository.Api;

public class Startup(IConfiguration configuration, ILogger logger)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.UseSqlDatabase(configuration);

        services.AddSettings(configuration);
        services.AddAuthentication(configuration);
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

        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        services.AddControllers()
            .AddControllersAsServices(); // this helps to check if any controllers have missing DIs;

        services.AddAuthorization();

        // Register the Swagger API documentation generator
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen().ConfigureOptions<SwaggerOptions>();
    }

    public static void Configure(WebApplication app)
    {
        app.UseSwagger();
        var majorVersion = app.Configuration.GetSection("AppSettings:Version").Value!.Split('.').First();
        app.UseSwaggerUI(options =>
        {
            options.DefaultModelsExpandDepth(-1);
            options.DefaultModelExpandDepth(-1);
            options.SwaggerEndpoint($"/swagger/v{majorVersion}/swagger.json", $"Recipe Repository v{majorVersion}");
        });

        app.UseCors();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // url for serving static files
        app.Map("/static", builder => { builder.UseFileServer(); });

        // set default url to swagger redirect
        app.MapGet("/", () => Results.Redirect("/swagger", true, true));
    }
}
