﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SelfHostingApiWithAuth.Auth.ApiKey.Abstraction;
using SelfHostingApiWithAuth.Auth.ApiKey.Models;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SelfHostingApiWithAuth.Auth.ApiKey.Services;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    // https://josef.codes/asp-net-core-protect-your-api-with-api-keys/

    private readonly IApiKeyOwnerService _apiKeyOwnerService;
    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IApiKeyOwnerService apiKeyOwnerService)
        : base(options, logger, encoder, clock)
    {
        _apiKeyOwnerService = apiKeyOwnerService ?? throw new ArgumentNullException(nameof(apiKeyOwnerService));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        //var endpoint = Request.HttpContext.Features.Get<IEndpointFeature>()?.Endpoint;
        //var allowAnonymous = endpoint?.Metadata.Any(x => x.GetType() == typeof(AllowAnonymousAttribute) || x.GetType() == typeof(AllowNoApiKeyAttribute)) ?? false;

        if (!Request.Headers.TryGetValue(ApiKeyConstants.HeaderName, out var apiKeyHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

        if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
        {
            return AuthenticateResult.NoResult();
        }

        var existingApiKey = await _apiKeyOwnerService.FindByKey(providedApiKey);

        if (existingApiKey != null)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, existingApiKey.OwnerId)
            };

            claims.AddRange(existingApiKey.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }

        return AuthenticateResult.NoResult();
    }
}