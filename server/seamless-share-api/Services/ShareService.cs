using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Services;

public class ShareService(ILogger<ShareService> logger)
{
    public async Task<ShareSchema> Create(Guid? ownerId, string? ipAddress, string? userAgent)
    {
        var share = new ShareSchema(ownerId, ipAddress, userAgent);
        await share.SaveAsync();

        return share;
    }
}
