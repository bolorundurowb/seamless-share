using meerkat;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Services;

public class LinkService(ILogger<LinkService> logger)
{
    public async Task<LinkSchema> Create(Guid shareId, string url, string? appVersion, AppSource? appSource)
    {
        logger.LogDebug("Creating a new shared link. {ShareId} {Url}", shareId, url);

        var link = new LinkSchema(shareId, url, appVersion, appSource);
        await link.SaveAsync();

        logger.LogDebug("Shared link successfully created. {LinkId} {Url}", link.Id, url);

        return link;
    }

    public Task<List<LinkSchema>> GetAll(Guid shareId) => Meerkat.Query<LinkSchema>()
        .Where(x => x.ShareId == shareId)
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();
}
