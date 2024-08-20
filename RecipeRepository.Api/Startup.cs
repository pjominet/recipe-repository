using System.Reflection;
using Microsoft.OpenApi.Models;
using RecipeRepository.Logic;

namespace RecipeRepository.Api;

public class Startup(IConfiguration configuration, ILogger logger)
{
    private readonly string _version = configuration.GetValue<string>("AppSettings:Version")!;

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
        services.AddSwaggerGen(gen =>
        {
            gen.SwaggerDoc(_version, new OpenApiInfo {Title = "Recipe Repository API", Version = _version});
            gen.CustomSchemaIds(selector => selector.FullName);
            gen.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        });
    }

    public void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseHsts();

        app.UseCors();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // url for serving static files
        app.Map("/static", builder => { builder.UseFileServer(); });

        // enable swagger
        app.UseSwagger();
        app.UseSwaggerUI(opt => { opt.SwaggerEndpoint($"/swagger/{_version}/swagger.json", $"Recipe Repository {_version}"); });

    }
}
