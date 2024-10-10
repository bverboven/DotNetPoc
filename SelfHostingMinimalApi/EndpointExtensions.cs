namespace SelfHostingMinimalApi;

public static class EndpointExtensions
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.AddDefaultMap();
        app.AddAboutMap();
    }

    public static RouteHandlerBuilder AddDefaultMap(this IEndpointRouteBuilder app)
        => app.MapGet("/", () => "Hello from self hosting api");
    public static RouteHandlerBuilder AddAboutMap(this IEndpointRouteBuilder app)
        => app.MapGet("/about", () => "About me");
}
