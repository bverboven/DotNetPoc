using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.Reflection;

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
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning);
        });

        return builder;
    }
}