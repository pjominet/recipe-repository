using RecipeRepository.Logic;

namespace RecipeRepository.Api;

public class Startup(IConfiguration configuration, ILogger logger)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.EnsureDatabase(configuration);

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

        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
        });

        services.AddAutoMapper(typeof(LogicAssemblyMarker).Assembly);

        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        services.AddControllers();

        services.AddAuthorization();

        // Register the Swagger API documentation generator
        services.AddSwaggerGen().ConfigureOptions<SwaggerOptions>();
    }

    public static void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseHsts();

        // enable swagger
        app.UseSwagger();
        var version = app.Configuration.GetSection("AppSettings:Version").Value;
        app.UseSwaggerUI(opt => { opt.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"Recipe Repository {version}"); });

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
