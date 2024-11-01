using SelfHostingApiWithAuth.Auth.ApiKey.Abstraction;
using SelfHostingApiWithAuth.Auth.ApiKey.Extensions;
using SelfHostingApiWithAuth.Auth.ApiKey.Models;

namespace SelfHostingApiWithAuth.Auth.ApiKey.Services;

public class InMemoryApiKeyOwnerService : IApiKeyOwnerService
{
    private readonly IList<ApiKeyOwner> _apiKeyOwners;
    public InMemoryApiKeyOwnerService(IEnumerable<ApiKeyOwner> apiKeyOwners)
    {
        _apiKeyOwners = apiKeyOwners.ToArray();
    }

    public Task<ApiKeyOwner?> FindByOwner(string id)
    {
        return Task.FromResult(_apiKeyOwners.FirstOrDefault(x => x.OwnerId.Equals(id, StringComparison.InvariantCultureIgnoreCase)));
    }
    public Task<ApiKeyOwner?> FindByKey(string apiKey)
    {
        return Task.FromResult(_apiKeyOwners.FirstOrDefault(x => x.Key == apiKey));
    }

    public async Task<bool> Validate(string id, string apiKey)
    {
        var owner = await FindByOwner(id);
        return owner != null && owner.Key == apiKey;
    }

    public static InMemoryApiKeyOwnerService FromConfigurationSection(IConfigurationSection configSection)
        => new(configSection.ToApiKeyOwners());
}