using meerkat;
using MongoDB.Driver.Linq;
using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Services;

public class ShareService(ILogger<ShareService> logger)
{
    public Task<ShareSchema?> GetOne(Guid shareId, Guid? ownerId) =>
        Meerkat.FindOneAsync<ShareSchema>(x => x.Id == (object)shareId && x.OwnerId == ownerId);

    public Task<ShareSchema?> GetOneByCode(string shareCode, Guid? ownerId)
    {
        logger.LogDebug("Getting share by code {ShareCode} {OwnerId}", shareCode, ownerId);
        return Meerkat.FindOneAsync<ShareSchema>(x => x.Code == shareCode && x.OwnerId == ownerId);
    }

    public async Task<ShareSchema?> GetOwned(Guid ownerId)
    {
        logger.LogDebug("Getting owned share {OwnerId}", ownerId);
        var shares = await Meerkat.Query<ShareSchema>()
            .Where(x => x.OwnerId == ownerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        if (shares.Count > 1)
            logger.LogError("A user with more than one owned share discovered. {OwnerId}, {ShareIds}", ownerId,
                string.Join(", ", shares.Select(x => x.Id)));

        return shares.FirstOrDefault();
    }

    public async Task<ShareSchema> Create(Guid? ownerId, string? ipAddress, string? userAgent)
    {
        logger.LogDebug("Creating a new share. {OwnerId} {IpAddress} {UserAgent}", ownerId, ipAddress, userAgent);
        var share = new ShareSchema(ownerId, ipAddress, userAgent);
        await share.SaveAsync();

        logger.LogDebug("Share successfully created. {ShareId} {IpAddress} {UserAgent}", share.Id, ipAddress,
            userAgent);

        return share;
    }

    public Task<bool> HasShareAccess(Guid shareId, Guid? ownerId) => Meerkat.Query<ShareSchema>()
        .AnyAsync(x => x.Id == (object)shareId && (x.OwnerId == null || x.OwnerId == ownerId));
}
