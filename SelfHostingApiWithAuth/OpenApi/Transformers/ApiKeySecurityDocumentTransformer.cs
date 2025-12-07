using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using SelfHostingApiWithAuth.Auth.ApiKey.Models;

namespace SelfHostingApiWithAuth.OpenApi.Transformers;

public class ApiKeySecurityDocumentTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == ApiKeyDefaults.AuthenticationScheme))
        {
            var scheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = ApiKeyDefaults.HeaderName,
                Description = $"API key required in the '{ApiKeyDefaults.HeaderName}' header"
            };
            var requirements = new Dictionary<string, IOpenApiSecurityScheme>
            {
                [ApiKeyDefaults.AuthenticationScheme] = scheme
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes.TryAdd(ApiKeyDefaults.AuthenticationScheme, scheme);
        }
    }
}