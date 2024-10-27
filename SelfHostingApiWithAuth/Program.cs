using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using SelfHostingApiWithAuth.ApiKey.Extensions;
using SelfHostingApiWithAuth.ApiKey.Models;
using SelfHostingApiWithAuth.Jwt.Extensions;
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
var secret = builder.Configuration["Authentication:Jwt:Secret"];
builder.Services.AddJwtAuthentication(o => o.Secret = secret ?? throw new NullReferenceException("Secret is missing"));

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

            return ApiKeyConstants.AuthenticationScheme;
        };
    });


builder.Services.AddAuthorization(options =>
{
    var schemes = new List<string>();
    // API
    schemes.Add(ApiKeyConstants.AuthenticationScheme);
    // JWT
    schemes.Add(JwtBearerDefaults.AuthenticationScheme);
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(schemes.ToArray())
        .Build();
});

// Swagger
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo { Title = applicationName, Version = $"v{version.Major}.{version.Minor}.{version.Build}" });

    s.AddJwtAuthentication();
    s.AddApiKeyAuthentication();

    var xmlFile = $"{applicationName}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        s.IncludeXmlComments(xmlPath, true);
    }
});

// Configure Application
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("swagger/v1/swagger.json", $"{applicationName} v{version.Major}.{version.Minor}");
        c.RoutePrefix = string.Empty;
    });
}

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
