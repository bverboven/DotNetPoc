using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using SelfHostingApiWithAuth.Auth.ApiKey.Abstraction;
using SelfHostingApiWithAuth.Auth.ApiKey.Models;
using SelfHostingApiWithAuth.Auth.ApiKey.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SelfHostingApiWithAuth.Auth.ApiKey.Extensions;

public static class ApiKeyExtensions
{
    public static AuthenticationBuilder AddApiKeyAuthentication(this IServiceCollection services, Action<ApiKeyAuthenticationOptions>? configure = null)
    {
        return services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ApiKeyConstants.AuthenticationScheme;
                options.DefaultChallengeScheme = ApiKeyConstants.AuthenticationScheme;
            })
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyConstants.AuthenticationScheme, configure ?? (c => { }));
    }

    public static AuthenticationBuilder AddInMemoryApiKeyAuthentication(this AuthenticationBuilder builder, IEnumerable<ApiKeyOwner> apiKeyOwners)
    {
        builder.Services
            .AddSingleton<IApiKeyOwnerService>(_ => new InMemoryApiKeyOwnerService(apiKeyOwners));

        return builder;
    }

    public static SwaggerGenOptions AddApiKeyAuthentication(this SwaggerGenOptions o, string authenticationScheme = ApiKeyConstants.AuthenticationScheme)
    {
        var apiKeySecurityScheme = new OpenApiSecurityScheme
        {
            Scheme = authenticationScheme,
            Name = ApiKeyConstants.HeaderName,
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Description = "ApiKey",

            Reference = new OpenApiReference
            {
                Id = authenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };

        o.AddSecurityDefinition(apiKeySecurityScheme.Reference.Id, apiKeySecurityScheme);
        o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {apiKeySecurityScheme, Array.Empty<string>()}
            });

        return o;
    }
}