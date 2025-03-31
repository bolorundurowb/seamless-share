using meerkat;
using MongoDB.Driver.Linq;
using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Services;

public class ImageService(ILogger<ImageService> logger)
{
    public async Task<ImageSchema> Create(Guid ownerId, string imageUrl, FileMetadata imageMetadata,
        string? appVersion, AppSource? appSource)
    {
        logger.LogDebug("Creating a new image. {OwnerId} {ImageUrl} {ImageMetadata}", ownerId, imageUrl,
            imageMetadata);

        var image = new ImageSchema(ownerId, imageMetadata, imageUrl, appVersion, appSource);
        await image.SaveAsync();

        logger.LogDebug("Image successfully persisted. {ImageId} {ImageUrl} {ImageMetadata}", image.Id, imageUrl,
            imageMetadata);

        return image;
    }

    public Task<List<ImageSchema>> GetAll(Guid shareId) => Meerkat.Query<ImageSchema, Guid>()
        .Where(x => x.ShareId == shareId && x.IsArchived == false)
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();

    public async Task DeleteOne(Guid shareId, Guid imageId)
    {
        logger.LogDebug("Request to delete an image. {ShareId} {ImageId}", shareId, imageId);

        var image = await Meerkat.Query<ImageSchema, Guid>()
            .FirstOrDefaultAsync(x => x.ShareId == shareId && x.Id == imageId);

        if (image is null)
        {
            logger.LogWarning("Image to be deleted does not exist. {ShareId} {ImageId}", shareId, imageId);
            return;
        }

        await Meerkat.RemoveByIdAsync<ImageSchema, Guid>(imageId);

        logger.LogDebug("Image successfully deleted. {ShareId} {ImageId}", shareId, imageId);
    }

    public async Task ArchiveOne(Guid shareId, Guid imageId)
    {
        logger.LogDebug("Request to archive an image. {ShareId} {ImageId}", shareId, imageId);

        var image = await Meerkat.Query<ImageSchema, Guid>()
            .FirstOrDefaultAsync(x => x.ShareId == shareId && x.Id == imageId);

        if (image is null)
        {
            logger.LogWarning("Image to be archived does not exist. {ShareId} {ImageId}", shareId, imageId);
            return;
        }

        image.Archive();
        await image.SaveAsync();

        logger.LogDebug("Image successfully archived. {ShareId} {ImageId}", shareId, imageId);
    }
}