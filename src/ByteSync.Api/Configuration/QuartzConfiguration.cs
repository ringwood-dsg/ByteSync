using ByteSync.Api.Jobs;
using Quartz;

namespace ByteSync.Api.Configuration;

public static class QuartzConfiguration
{
    public static void ConfigureJobs(IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            var refreshAnnouncementsJobKey = new JobKey("RefreshAnnouncementsJob");
            q.AddJob<RefreshAnnouncementsJob>(opts => opts.WithIdentity(refreshAnnouncementsJobKey));
            q.AddTrigger(opts => opts
                .ForJob(refreshAnnouncementsJobKey)
                .WithIdentity("RefreshAnnouncementsJob-trigger")
                .WithCronSchedule("0 0 */2 * * ?"));

            var cleanupAzureBlobStorageJobKey = new JobKey("CleanupAzureBlobStorageSnippetsJob");
            q.AddJob<CleanupAzureBlobStorageSnippetsJob>(opts => opts.WithIdentity(cleanupAzureBlobStorageJobKey));
            q.AddTrigger(opts => opts
                .ForJob(cleanupAzureBlobStorageJobKey)
                .WithIdentity("CleanupAzureBlobStorageSnippetsJob-trigger")
                .WithCronSchedule("0 0 0 * * ?"));

            var cleanupCloudflareR2JobKey = new JobKey("CleanupCloudflareR2SnippetsJob");
            q.AddJob<CleanupCloudflareR2SnippetsJob>(opts => opts.WithIdentity(cleanupCloudflareR2JobKey));
            q.AddTrigger(opts => opts
                .ForJob(cleanupCloudflareR2JobKey)
                .WithIdentity("CleanupCloudflareR2SnippetsJob-trigger")
                .WithCronSchedule("0 0 0 * * ?"));
        });
    }
}
