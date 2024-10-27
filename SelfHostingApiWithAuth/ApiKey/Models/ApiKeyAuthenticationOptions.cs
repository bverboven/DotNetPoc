using Microsoft.AspNetCore.Authentication;

namespace SelfHostingApiWithAuth.ApiKey.Models;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Scheme => ApiKeyConstants.AuthenticationScheme;
    public string AuthenticationType = ApiKeyConstants.AuthenticationScheme;
}