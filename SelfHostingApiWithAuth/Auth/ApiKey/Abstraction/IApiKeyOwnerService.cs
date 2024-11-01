using SelfHostingApiWithAuth.Auth.ApiKey.Models;

namespace SelfHostingApiWithAuth.Auth.ApiKey.Abstraction;

public interface IApiKeyOwnerService
{
    public Task<ApiKeyOwner?> FindByOwner(string id);
    public Task<ApiKeyOwner?> FindByKey(string apiKey);

    public Task<bool> Validate(string id, string apiKey);
}