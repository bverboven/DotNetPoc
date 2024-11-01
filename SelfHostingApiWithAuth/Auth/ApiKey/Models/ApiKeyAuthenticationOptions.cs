using Microsoft.AspNetCore.Authentication;

namespace SelfHostingApiWithAuth.Auth.ApiKey.Models;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Scheme => ApiKeyConstants.AuthenticationScheme;
    public string AuthenticationType = ApiKeyConstants.AuthenticationScheme;
}