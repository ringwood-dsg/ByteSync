using ByteSync.Common.Business.Announcements;
using ByteSync.ServerCommon.Interfaces.Loaders;
using ByteSync.ServerCommon.Interfaces.Repositories;
using Quartz;

namespace ByteSync.Api.Jobs;

public class RefreshAnnouncementsJob : IJob
{
    private readonly IAnnouncementsLoader _loader;
    private readonly IAnnouncementRepository _repository;
    private readonly ILogger<RefreshAnnouncementsJob> _logger;

    public RefreshAnnouncementsJob(IAnnouncementsLoader loader, IAnnouncementRepository repository,
        ILogger<RefreshAnnouncementsJob> logger)
    {
        _loader = loader;
        _repository = repository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var currentUtcTime = DateTime.UtcNow;
        _logger.LogInformation("Refreshing announcements at: {Now}", currentUtcTime);

        var announcements = await _loader.Load();
        var validAnnouncements = announcements.Where(d => d.EndDate > currentUtcTime).ToList();

        await _repository.SaveAll(validAnnouncements);

        _logger.LogInformation("Refreshed {Count} announcements", validAnnouncements.Count);
    }
}
