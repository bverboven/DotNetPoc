using Microsoft.Extensions.Hosting.WindowsServices;
using Scalar.AspNetCore;
using SelfHostingMinimalApi;

var builder = WebApplication.CreateBuilder(args);

if (WindowsServiceHelpers.IsWindowsService())
{
    builder.Host.UseWindowsService();
}

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});

// OpenApi
builder.Services.AddOpenApi();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
    app.MapScalarApiReference();
}

// Endpoints
app.AddEndpoints();

// Adds (un)install.bat script to (un)register as Windows service
var host = app.Services.GetRequiredService<IHost>();
host.AddWindowsServiceInstaller("POC Self Hosting API");

app.Run();
