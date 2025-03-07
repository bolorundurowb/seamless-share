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

    public Task<List<LinkSchema>> GetAll(Guid shareId) => Meerkat.Query<LinkSchema, Guid>()
        .Where(x => x.ShareId == shareId)
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();

    public async Task DeleteOne(Guid shareId, Guid linkId)
    {
        logger.LogDebug("Deleting a shared link. {ShareId} {LinkId}", shareId, linkId);

        await Meerkat.RemoveOneAsync<LinkSchema, Guid>(x => x.ShareId == shareId && x.Id == linkId);

        logger.LogDebug("Shared link successfully deleted. {ShareId} {LinkId}", shareId, linkId);
    }

    public async Task ArchiveOne(Guid shareId, Guid linkId)
    {
        logger.LogDebug("Archiving a shared link. {ShareId} {LinkId}", shareId, linkId);

        var link = await Meerkat.FindOneAsync<LinkSchema, Guid>(x => x.ShareId == shareId && x.Id == linkId);

        if (link is null)
        {
            logger.LogWarning("Link to be archived does not exist. {LinkId}", linkId);
            return;
        }

        link.Archive();
        await link.SaveAsync();

        logger.LogDebug("Shared link successfully deleted. {ShareId} {LinkId}", shareId, linkId);
    }
}