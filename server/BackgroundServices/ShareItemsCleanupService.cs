using meerkat;
using MongoDB.Driver.Linq;
using SeamlessShareApi.Models.Data;
using SeamlessShareApi.Services;

namespace SeamlessShareApi.BackgroundServices;

public class ShareItemsCleanupService : BackgroundService
{
    private const int MaxAnonymousShareAgeInDays = 7;
    private const int MaxOwnedShareAgeInDays = 30;

    private readonly ILogger<ShareItemsCleanupService> _logger;
    private readonly FileService _fileService;

    private readonly TimeSpan[] _scheduledTimes;
    private DateTime _nextRunTime;

    public ShareItemsCleanupService(ILogger<ShareItemsCleanupService> logger, FileService fileService)
    {
        _logger = logger;
        _fileService = fileService;

        _scheduledTimes =
        [
            TimeSpan.FromHours(6), // 6:00 AM
            TimeSpan.FromHours(23) // 11:00 PM
        ];
        CalculateNextRunTime();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ShareItemsCleanupService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            if (now > _nextRunTime)
            {
                try
                {
                    _logger.LogInformation("Starting scheduled cleanup work at {time}", now);

                    // find eligible shares
                    var anonymousShareIds = await GetAnonymousShareIds();
                    var ownedShareIds = await GetOwnedShareIds();

                    // clean up items
                    await DeleteExpiredLinks(anonymousShareIds, ownedShareIds);
                    await DeleteExpiredTexts(anonymousShareIds, ownedShareIds);
                    await DeleteOExpiredImages(anonymousShareIds, ownedShareIds);
                    await DeleteExpiredDocuments(anonymousShareIds, ownedShareIds);

                    _logger.LogInformation("Scheduled cleanup work completed at {time}", DateTime.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing scheduled work");
                }

                CalculateNextRunTime();
            }

            // Wait for 1 minute before checking again
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogInformation("ShareItemsCleanupService is exiting.");
    }

    private async Task DeleteExpiredLinks(List<Guid> anonymousShareIds, List<Guid> ownedShareIds)
    {
        _logger.LogInformation("Cleaning up expired links.");
        await Meerkat.RemoveAsync<LinkSchema, Guid>(x =>
            x.CreatedAt < DateTime.UtcNow.AddDays(-MaxAnonymousShareAgeInDays) && anonymousShareIds.Contains(x.Id));
        await Meerkat.RemoveAsync<LinkSchema, Guid>(x =>
            x.CreatedAt < DateTime.UtcNow.AddDays(-MaxOwnedShareAgeInDays) && ownedShareIds.Contains(x.Id));
        _logger.LogInformation("Expired links cleaned up.");
    }

    private async Task DeleteExpiredTexts(List<Guid> anonymousShareIds, List<Guid> ownedShareIds)
    {
        _logger.LogInformation("Cleaning up expired texts.");
        await Meerkat.RemoveAsync<TextSchema, Guid>(x =>
            x.CreatedAt < DateTime.UtcNow.AddDays(-MaxAnonymousShareAgeInDays) && anonymousShareIds.Contains(x.Id));
        await Meerkat.RemoveAsync<TextSchema, Guid>(x =>
            x.CreatedAt < DateTime.UtcNow.AddDays(-MaxOwnedShareAgeInDays) && ownedShareIds.Contains(x.Id));
        _logger.LogInformation("Expired texts cleaned up.");
    }

    private async Task DeleteOExpiredImages(List<Guid> anonymousShareIds, List<Guid> ownedShareIds)
    {
        _logger.LogInformation("Cleaning up expired images.");

        var anonymousImages = await Meerkat.FindAsync<ImageSchema, Guid>(x =>
            x.CreatedAt < DateTime.UtcNow.AddDays(-MaxAnonymousShareAgeInDays) && anonymousShareIds.Contains(x.Id));
        var ownedImages = await Meerkat.FindAsync<ImageSchema, Guid>(x =>
            x.CreatedAt < DateTime.UtcNow.AddDays(-MaxOwnedShareAgeInDays) && ownedShareIds.Contains(x.Id));

        // delete files from external provider
        await Task.WhenAll(anonymousImages.Select(x => _fileService.Delete(x.Metadata.ExternalId)));
        await Task.WhenAll(ownedImages.Select(x => _fileService.Delete(x.Metadata.ExternalId)));

        // delete images from database
        await Meerkat.RemoveAsync<ImageSchema, Guid>(x =>
            x.CreatedAt < DateTime.UtcNow.AddDays(-MaxAnonymousShareAgeInDays) && anonymousShareIds.Contains(x.Id));
        await Meerkat.RemoveAsync<ImageSchema, Guid>(x =>
            x.CreatedAt < DateTime.UtcNow.AddDays(-MaxOwnedShareAgeInDays) && ownedShareIds.Contains(x.Id));

        _logger.LogInformation("Expired images cleaned up.");
    }

    private async Task DeleteExpiredDocuments(List<Guid> anonymousShareIds, List<Guid> ownedShareIds)
    {
        _logger.LogInformation("Cleaning up expired documents.");
        var anonymousDocuments = await Meerkat.FindAsync<DocumentSchema, Guid>(x =>
            x.CreatedAt < DateTime.UtcNow.AddDays(-MaxAnonymousShareAgeInDays) && anonymousShareIds.Contains(x.Id));
        var ownedDocuments = await Meerkat.FindAsync<DocumentSchema, Guid>(x =>
            x.CreatedAt < DateTime.UtcNow.AddDays(-MaxOwnedShareAgeInDays) && ownedShareIds.Contains(x.Id));

        // delete files from external provider
        await Task.WhenAll(anonymousDocuments.Select(x => _fileService.Delete(x.Metadata.ExternalId)));
        await Task.WhenAll(ownedDocuments.Select(x => _fileService.Delete(x.Metadata.ExternalId)));

        // delete documents from database
        await Meerkat.RemoveAsync<DocumentSchema, Guid>(x =>
            x.CreatedAt < DateTime.UtcNow.AddDays(-MaxAnonymousShareAgeInDays) && anonymousShareIds.Contains(x.Id));
        await Meerkat.RemoveAsync<DocumentSchema, Guid>(x =>
            x.CreatedAt < DateTime.UtcNow.AddDays(-MaxOwnedShareAgeInDays) && ownedShareIds.Contains(x.Id));

        _logger.LogInformation("Expired documents cleaned up.");
    }

    private Task<List<Guid>> GetAnonymousShareIds() => Meerkat.Query<ShareSchema, Guid>()
        .Where(x => x.OwnerId == null)
        .Select(x => x.Id)
        .ToListAsync();

    private Task<List<Guid>> GetOwnedShareIds() => Meerkat.Query<ShareSchema, Guid>()
        .Where(x => x.OwnerId != null && x.OwnerId != Guid.Empty)
        .Select(x => x.Id)
        .ToListAsync();

    private void CalculateNextRunTime()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;

        // Find all future times today
        var futureTimesToday = _scheduledTimes
            .Select(time => today.Add(time))
            .Where(time => time > now)
            .ToList();

        _nextRunTime = futureTimesToday.Any()
            ? futureTimesToday.Min()
            :
            // If no more times today, use first time tomorrow
            today.AddDays(1).Add(_scheduledTimes[0]);

        _logger.LogInformation("Next run scheduled for {nextRunTime}", _nextRunTime);
    }
}
