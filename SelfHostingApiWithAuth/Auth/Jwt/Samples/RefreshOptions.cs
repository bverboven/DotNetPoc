namespace SelfHostingApiWithAuth.Auth.Jwt.Samples;

public class RefreshOptions
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? AuthenticationScheme { get; set; }
}