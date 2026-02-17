using ByteSync.ServerCommon.Commands.Storage;
using MediatR;
using Quartz;

namespace ByteSync.Api.Jobs;

public class CleanupAzureBlobStorageSnippetsJob : IJob
{
    private readonly ILogger<CleanupAzureBlobStorageSnippetsJob> _logger;
    private readonly IMediator _mediator;

    public CleanupAzureBlobStorageSnippetsJob(IMediator mediator, 
        ILogger<CleanupAzureBlobStorageSnippetsJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Cleanup Azure BlobStorage Job started at: {Now}", DateTime.UtcNow);
        var deletedBlobsCount = await _mediator.Send(new CleanupAzureBlobStorageSnippetsRequest());
        _logger.LogInformation("Cleanup Azure BlobStorage Job - Deletion complete, {Deleted} element(s)", deletedBlobsCount);
    }
}
