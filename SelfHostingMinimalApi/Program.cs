var builder = WebApplication.CreateBuilder(args);

// configure port
var port = builder.Configuration["Hosting:Port"] ?? "9000";
builder.Configuration
    .AddConfiguration(new ConfigurationBuilder()
            // this config replaces .UseUrls from the webBuilder in ConfigureWebHostDefaults
            .AddInMemoryCollection(new Dictionary<string, string> { ["urls"] = $"http://*:{port}" }!)
            .Build()
    );

builder.Services.AddWindowsService();

var app = builder.Build();
// Endpoints
app.MapGet("/", () => "Hello from self hosting api");

// Adds (un)install.bat script to (un)register as Windows service
var host = app.Services.GetRequiredService<IHost>();
host.AddWindowsServiceInstaller("POC Self Hosting API");

app.Run();
