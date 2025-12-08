using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Regira.ConsoleWithLogging;

public static class HostExtensions
{
    public static IHostBuilder AddConfiguration(this IHostBuilder builder)
    {
        return builder.ConfigureAppConfiguration((_, config) =>
        {
            config.Sources.Clear();
            // add configuration
            config
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", true, true)
#if DEBUG
                .AddUserSecrets(typeof(Program).Assembly, true)
#endif
                ;
        });
    }
    public static IHostBuilder AddServices(this IHostBuilder builder)
    {
        return builder.ConfigureServices((context, services) =>
        {
            var config = context.Configuration;

            // register services here
        });
    }

    public static IHostBuilder AddSerilog(this IHostBuilder builder)
    {
        builder.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });

        return builder;
    }
}