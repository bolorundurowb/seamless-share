using meerkat;
using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Services;

public class ShareService(ILogger<ShareService> logger)
{
    public Task<ShareSchema?> GetOne(Guid shareId, Guid? ownerId) =>
        Meerkat.FindOneAsync<ShareSchema>(x => x.Id == (object)shareId && x.OwnerId == ownerId);

    public async Task<ShareSchema> Create(Guid? ownerId, string? ipAddress, string? userAgent)
    {
        var share = new ShareSchema(ownerId, ipAddress, userAgent);
        await share.SaveAsync();

        return share;
    }
}
