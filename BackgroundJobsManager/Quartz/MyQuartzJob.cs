using Microsoft.Extensions.Logging;
using Quartz;

namespace Regira.BackgroundJobsManager.Quartz;

public class MyQuartzJob(ILogger<MyQuartzJob> logger) : IJob
{
    public Task Execute(IJobExecutionContext _)
    {
        logger.LogInformation($"Quartz: {DateTime.Now:HH:mm:ss.fff}");
        return Task.CompletedTask;
    }
}
