namespace SeamlessShareApi.Models.Data;

public abstract class BaseFileSchema : BaseShareItemSchema
{
    public Guid ShareId { get; private set; }

    public FileMetadata Metadata { get; private set; }

    public string Url { get; private set; }

    protected BaseFileSchema() { }

    public BaseFileSchema(Guid shareId, FileMetadata metadata, string url, string? appVersion, AppSource? appSource)
    {
        Source = appSource;
        AppVersion = appVersion;

        Id = Guid.NewGuid();
        ShareId = shareId;
        Metadata = metadata;
        Url = url;
        IsArchived = false;
    }
}

public record FileMetadata(
    string ExternalId,
    string Name,
    string Extension,
    long SizeInBytes,
    string MimeType,
    string? Checksum
);
