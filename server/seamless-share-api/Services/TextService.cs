using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Services;

public class TextService(ILogger<TextService> logger)
{
    public async Task<TextSchema> Create(Guid shareId, string content)
    {
        logger.LogDebug("Creating a new shared text. {ShareId} {Content}", shareId, content);

        var text = new TextSchema(shareId, content);
        await text.SaveAsync();

        logger.LogDebug("Shared text successfully created. {TextId} {Content}", text.Id, content);

        return text;
    }
}
