using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Regira.BackgroundJobsManager.Quartz;

public static class QuartzExtensions
{
    public static IHostBuilder ConfigureQuartzJobs(this IHostBuilder builder)
    {
        return builder.ConfigureServices((context, services) =>
        {
            services
                .AddQuartzJobs()
                .AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        });
    }

    public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
    {
        return services.AddQuartz(q =>
        {
            q.SchedulerName = "MyQuartzScheduler";

            q.ScheduleJob<MyQuartzJob>(trigger => trigger
                .ForJob(nameof(MyQuartzJob))
                .WithIdentity($"{nameof(MyQuartzJob)}-trigger")
                .WithCronSchedule("*/10 * * ? * 1-7 *")
            );
        });
    }

    //public static async Task AddQuartzJobs(this IHost host)
    //{
    //    var schedulerFactory = host.Services.GetRequiredService<ISchedulerFactory>();
    //    var scheduler = await schedulerFactory.GetScheduler();
    //    await scheduler.ScheduleJob(JobBuilder.Create<MyQuartzJob>().Build(), TriggerBuilder.Create()
    //        .WithIdentity($"{nameof(MyQuartzJob)}-trigger")
    //        .StartNow()
    //        .WithCronSchedule("*/10 * * ? * 1-7 *")
    //        .Build()
    //    );
    //}
}
