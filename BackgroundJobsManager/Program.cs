using Microsoft.Extensions.Hosting;
using Regira.BackgroundJobsManager;
using Regira.BackgroundJobsManager.Hangfire;
using Regira.BackgroundJobsManager.Quartz;


Console.WriteLine("Started");

var builder = Host.CreateDefaultBuilder(args)
    .AddConfiguration()
    .AddServices()
    .ConfigureQuartzJobs()
    .ConfigureHangfire();
var host = builder.Build();

host.Start();

//await host.AddQuartzJobs();
host.AddHangfireJobs();

await host.RunAsync();

Console.WriteLine("Finished");
