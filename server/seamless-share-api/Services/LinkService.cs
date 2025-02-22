using meerkat;
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

    public async Task DeleteOne(Guid shareId, Guid linkId)
    {
        logger.LogDebug("Deleting a shared link. {ShareId} {LinkId}", shareId, linkId);

        await Meerkat.RemoveOneAsync<LinkSchema>(x => x.ShareId == shareId && x.Id == (object)linkId);

        logger.LogDebug("Shared link successfully deleted. {ShareId} {LinkId}", shareId, linkId);
    }
}