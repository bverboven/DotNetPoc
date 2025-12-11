using Microsoft.OpenApi;
using SelfHostingApiWithAuth.Auth.ApiKey.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SelfHostingApiWithAuth.Auth.Swagger;

public static class SwaggerAuthExtensions
{
    public static void AddSwaggerWithAuth(this IServiceCollection services)
    {
        services
            .AddSwaggerGen(options =>
            {
                options
                    .AddApiKeyScheme()
                    .AddBearerScheme();
            });
    }
    public static SwaggerGenOptions AddApiKeyScheme(this SwaggerGenOptions options)
    {
        var apiKeySecurityScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Scheme = ApiKeyDefaults.AuthenticationScheme,
            Name = ApiKeyDefaults.HeaderName,
            Description = $"API key required in the '{ApiKeyDefaults.HeaderName}' header"
        };

        options.AddSecurityDefinition(ApiKeyDefaults.AuthenticationScheme, apiKeySecurityScheme);
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(ApiKeyDefaults.AuthenticationScheme, document)] = []
        });

        return options;
    }
    public static SwaggerGenOptions AddBearerScheme(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme."
        });
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("bearer", document)] = []
        });

        return options;
    }

    public static void UseSwaggerWithAuth(this IApplicationBuilder app)
    {
        app.UseSwagger(options => options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1);
        app.UseSwaggerUI();
    }
}