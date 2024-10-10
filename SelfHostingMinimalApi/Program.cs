using Microsoft.Extensions.Hosting.WindowsServices;
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


var app = builder.Build();
// Endpoints
app.AddEndpoints();

// Adds (un)install.bat script to (un)register as Windows service
var host = app.Services.GetRequiredService<IHost>();
host.AddWindowsServiceInstaller("POC Self Hosting API");

app.Run();
