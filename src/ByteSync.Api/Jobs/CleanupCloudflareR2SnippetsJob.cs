using ByteSync.ServerCommon.Commands.Storage;
using MediatR;
using Quartz;

namespace ByteSync.Api.Jobs;

public class CleanupCloudflareR2SnippetsJob : IJob
{
    private readonly ILogger<CleanupCloudflareR2SnippetsJob> _logger;
    private readonly IMediator _mediator;

    public CleanupCloudflareR2SnippetsJob(IMediator mediator, 
        ILogger<CleanupCloudflareR2SnippetsJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Cleanup Cloudflare R2 Job started at: {Now}", DateTime.Now);
        var deletedCount = await _mediator.Send(new CleanupCloudflareR2SnippetsRequest());
        _logger.LogInformation("Cleanup Cloudflare R2 Job - Deletion complete, {Deleted} element(s)", deletedCount);
    }
}
