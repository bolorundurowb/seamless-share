using meerkat.Attributes;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "files", TrackTimestamps = true)]
public class ImageSchema : BaseFileSchema
{
    protected ImageSchema() { }

    public ImageSchema(Guid shareId, FileMetadata metadata, string url, string? appVersion, AppSource? appSource) :
        base(shareId, metadata, url, appVersion, appSource) { }
}
