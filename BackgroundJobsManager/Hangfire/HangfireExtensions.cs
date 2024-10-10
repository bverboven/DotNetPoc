using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Regira.BackgroundJobsManager.Hangfire;

public static class HangfireExtensions
{
    public static IHostBuilder ConfigureHangfire(this IHostBuilder builder)
    {
        return builder.ConfigureServices((context, services) =>
        {
            services.AddTransient<MyHangfireJob>();

            services.AddHangfire((provider, configuration) => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseInMemoryStorage());

            services.AddHangfireServer(options =>
            {
                options.StopTimeout = TimeSpan.FromSeconds(15);
                options.ShutdownTimeout = TimeSpan.FromSeconds(30);
            });
        });
    }

    public static void AddHangfireJobs(this IHost app)
    {
        RecurringJob.AddOrUpdate<MyHangfireJob>("MyHangfireJob", x => x.Execute(), "*/10 * * * * *");
    }
}
