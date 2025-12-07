using Microsoft.AspNetCore.Authentication;
using SelfHostingApiWithAuth.Auth.ApiKey.Abstraction;
using SelfHostingApiWithAuth.Auth.ApiKey.Models;
using SelfHostingApiWithAuth.Auth.ApiKey.Services;

namespace SelfHostingApiWithAuth.Auth.ApiKey.Extensions;

public static class ApiKeyExtensions
{
    public static AuthenticationBuilder AddApiKeyAuthentication(this IServiceCollection services, Action<ApiKeyAuthenticationOptions>? configure = null)
    {
        return services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ApiKeyDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = ApiKeyDefaults.AuthenticationScheme;
            })
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyDefaults.AuthenticationScheme, configure ?? (_ => { }));
    }

    public static AuthenticationBuilder AddInMemoryApiKeyAuthentication(this AuthenticationBuilder builder, IEnumerable<ApiKeyOwner> apiKeyOwners)
    {
        builder.Services
            .AddSingleton<IApiKeyOwnerService>(_ => new InMemoryApiKeyOwnerService(apiKeyOwners));

        return builder;
    }
}