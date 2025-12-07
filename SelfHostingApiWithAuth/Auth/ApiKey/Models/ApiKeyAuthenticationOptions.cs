using Microsoft.AspNetCore.Authentication;

namespace SelfHostingApiWithAuth.Auth.ApiKey.Models;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Scheme => ApiKeyDefaults.AuthenticationScheme;
    public string AuthenticationType { get; set; } = ApiKeyDefaults.AuthenticationScheme;
    public string ApiKeyHeaderName { get; set; } = ApiKeyDefaults.HeaderName;
}