using meerkat.Attributes;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "documents", TrackTimestamps = true)]
public class DocumentSchema : BaseShareItemSchema
{
    public Guid ShareId { get; private set; }

    public FileMetadata Metadata { get; private set; }

    public string Url { get; private set; }

    private DocumentSchema() { }

    public DocumentSchema(Guid shareId, FileMetadata metadata, string url, string? appVersion, AppSource? appSource)
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
