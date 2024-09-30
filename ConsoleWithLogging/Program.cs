using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Regira.ConsoleWithLogging;
using Serilog;

// basic logger (configuration and services not loaded yet)
Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

try
{
    // configuration
    var host = Host.CreateDefaultBuilder(args)
        .AddConfiguration()
        .AddServices()
        .AddSerilog()
        .Build();

    // initialization
    using var scope = host.Services.CreateScope();
    var p = scope.ServiceProvider;

    var logger = p.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Start");
    Console.WriteLine();

    // Execute code here
    try
    {
        logger.LogDebug("Executing some code");
        throw new Exception("Forced error");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Oops, an error occured");
    }

    Console.WriteLine();
    logger.LogInformation("Finished");
}
catch (Exception ex)
{
    // Host error, logger might not be instanciated
    Log.Error(ex, "Host failed");
}
finally
{
    Console.WriteLine("Press enter to exit");
    Console.ReadLine();

    Log.CloseAndFlush();
}
