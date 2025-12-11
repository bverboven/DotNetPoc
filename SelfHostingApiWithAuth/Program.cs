using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using SelfHostingApiWithAuth.Auth.ApiKey.Extensions;
using SelfHostingApiWithAuth.Auth.ApiKey.Models;
using SelfHostingApiWithAuth.Auth.Jwt.Extensions;
using SelfHostingApiWithAuth.Auth.Swagger;
using SelfHostingApiWithAuth.OpenApi.Transformers;
using System.Reflection;

// Configure Services
var builder = WebApplication.CreateBuilder(args);

// configure Kestrel
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});

var applicationName = builder.Environment.ApplicationName;
var appAssembly = Assembly.Load(new AssemblyName(applicationName));
var version = appAssembly.GetName().Version ?? new Version(1, 0, 0);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Authentication (ApiKEY)
var authBuilder = builder.Services
    .AddApiKeyAuthentication()
    .AddInMemoryApiKeyAuthentication(builder.Configuration.GetSection("Authentication:ApiKeys").ToApiKeyOwners());
// Authentication (JWT)
builder.Services.AddJwtAuthentication(o =>
{
    var secret = builder.Configuration["Authentication:Jwt:Secret"];
    o.Secret = secret ?? throw new NullReferenceException("Secret is missing");
});

authBuilder
    .AddPolicyScheme("Smart", "Authorization Bearer or ApiKey", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader?.StartsWith("Bearer ") == true)
            {
                return JwtBearerDefaults.AuthenticationScheme;
            }

            return ApiKeyDefaults.AuthenticationScheme;
        };
    });


builder.Services.AddAuthorization(options =>
{
    var schemes = new List<string>();
    // API
    schemes.Add(ApiKeyDefaults.AuthenticationScheme);
    // JWT
    schemes.Add(JwtBearerDefaults.AuthenticationScheme);
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(schemes.ToArray())
        .Build();
});

// OpenAPI
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<ApiKeySecurityDocumentTransformer>();
    options.AddDocumentTransformer<BearerSecurityDocumentTransformer>();
});

// Swagger
builder.Services
    .AddSwaggerWithAuth();


// Configure Application
var app = builder.Build();

//if (app.Environment.IsDevelopment())
{
    // OpenApi
    app
        .MapOpenApi()
        .AllowAnonymous();

    // Scalar
    app.MapScalarApiReference(options =>
    {
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecuritySchemes = [ApiKeyDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme]
        };
    });

    // Swagger
    app.UseSwaggerWithAuth();
}

// Force https
//app.UseHttpsRedirection();

app
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseEndpoints(endpoints =>
    {
        var endpointBuilder = endpoints.MapControllers();
        // Force authentication by default on all requests
        endpointBuilder.RequireAuthorization();
    });

app.Run();
