using meerkat;
using SeamlessShareApi.Models.Data;
using MongoDB.Driver.Linq;

namespace SeamlessShareApi.Services;

public class TextService(ILogger<TextService> logger)
{
    public async Task<TextSchema> Create(Guid shareId, string content, string? appVersion, AppSource? appSource)
    {
        logger.LogDebug("Creating a new shared text. {ShareId} {Content}", shareId, content);

        var text = new TextSchema(shareId, content, appVersion, appSource);
        await text.SaveAsync();

        logger.LogDebug("Shared text successfully created. {TextId} {Content}", text.Id, content);

        return text;
    }

    public Task<List<TextSchema>> GetAll(Guid shareId) => Meerkat.Query<TextSchema, Guid>()
        .Where(x => x.ShareId == shareId)
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();

    public async Task DeleteOne(Guid shareId, Guid textId)
    {
        logger.LogDebug("Deleting shared text. {ShareId} {TextId}", shareId, textId);

        await Meerkat.RemoveOneAsync<TextSchema, Guid>(x => x.ShareId == shareId && x.Id == textId);

        logger.LogDebug("Shared text successfully deleted. {TextId}", textId);
    }

    public async Task ArchiveOne(Guid shareId, Guid textId)
    {
        logger.LogDebug("Archiving a shared text. {ShareId} {TextId}", shareId, textId);

        var text = await Meerkat.FindOneAsync<TextSchema, Guid>(x => x.ShareId == shareId && x.Id == textId);

        if (text is null)
        {
            logger.LogWarning("Text to be archived does not exist. {TextId}", textId);
            return;
        }

        text.Archive();
        await text.SaveAsync();

        logger.LogDebug("Shared text successfully deleted. {ShareId} {TextId}", shareId, textId);
    }
}