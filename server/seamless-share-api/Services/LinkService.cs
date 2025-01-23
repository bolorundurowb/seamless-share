using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Services;

public class LinkService(ILogger<LinkService> logger)
{
    public async Task<LinkSchema> Create(Guid shareId, string url)
    {
        logger.LogDebug("Creating a new shared link. {ShareId} {Url}", shareId, url);

        var link = new LinkSchema(shareId, url);
        await link.SaveAsync();

        logger.LogDebug("Shared link successfully created. {LinkId} {Url}", link.Id, url);

        return link;
    }
}
