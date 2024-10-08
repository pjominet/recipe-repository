﻿using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RecipeRepository.Logic.Infrastructure.Settings;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RecipeRepository.Api;

public class SwaggerOptions(IOptions<AppSettings> appOptions) : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly AppSettings _appSettings = appOptions.Value;

    public void Configure(SwaggerGenOptions options)
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

        options.SwaggerDoc($"v{_appSettings.Version.Split('.').First()}", new OpenApiInfo { Title = "Recipe Repository API", Version = _appSettings.Version });
        options.CustomSchemaIds(selector => selector.FullName);

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            Name = "Authorization",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT token to access Recipe Repository API",
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "JWT"
                    }
                },
                []
            }
        });
    }

    public void Configure(string? name, SwaggerGenOptions options) => Configure(options);
}
