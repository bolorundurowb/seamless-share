using meerkat;
using SeamlessShareApi.Models.Data;
using MongoDB.Driver;
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

    public Task<List<TextSchema>> GetAll(Guid shareId) => Meerkat.Query<TextSchema>()
        .Where(x => x.ShareId == shareId)
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();

    public async Task DeleteOne(Guid shareId, Guid textId)
    {
        logger.LogDebug("Deleting shared text. {ShareId} {TextId}", shareId, textId);

        await Meerkat.RemoveOneAsync<TextSchema>(x => x.ShareId == shareId && x.Id == (object)textId);

        logger.LogDebug("Shared text successfully deleted. {TextId}", textId);
    }
}