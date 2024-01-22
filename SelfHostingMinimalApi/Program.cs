using Regira.System.Hosting.WindowsService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWindowsService();

// configure port
var port = builder.Configuration["Hosting:Port"] ?? "9000";
builder.Configuration
    .AddConfiguration(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { ["urls"] = $"http://*:{port}" }!)
            .Build() // this config replaces .UseUrls from the webBuilder in ConfigureWebHostDefaults
    );


var app = builder.Build();
// Endpoints
app.MapGet("/", () => "Hello from self hosting api");

// Adds (un)install.bat script to (un)register as Windows service
var host = app.Services.GetRequiredService<IHost>();
host.AddWindowsServiceInstaller(new WindowsServiceOptions { ServiceName = "POC Self Hosting API" });

app.Run();