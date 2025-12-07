using SelfHostingApiWithAuth.Auth.ApiKey.Models;

namespace SelfHostingApiWithAuth.Auth.ApiKey.Extensions;

public static class ConfigurationExtensions
{
    public static IList<ApiKeyOwner> ToApiKeyOwners(this IConfigurationSection apiKeysSection)
    {
        return apiKeysSection
            .GetChildren()
            .Select(ToApiKeyOwner)
            .ToList();
    }
    public static ApiKeyOwner ToApiKeyOwner(this IConfigurationSection apiKeySection)
    {
        return new ApiKeyOwner
        {
            Key = apiKeySection["Key"],
            OwnerId = apiKeySection["OwnerId"]
        };
    }
}