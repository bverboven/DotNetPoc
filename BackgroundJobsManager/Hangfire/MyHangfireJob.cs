using Microsoft.Extensions.Logging;
using Regira.BackgroundJobsManager.Quartz;

namespace Regira.BackgroundJobsManager.Hangfire;

public class MyHangfireJob(ILogger<MyHangfireJob> logger)
{
    public Task Execute()
    {
        logger.LogInformation($"Hangfire: {DateTime.Now:HH:mm:ss.fff}");
        return Task.CompletedTask;
    }
}