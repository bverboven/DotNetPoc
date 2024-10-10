using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Regira.BackgroundJobsManager;

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
}
