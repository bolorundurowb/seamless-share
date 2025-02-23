using meerkat.Attributes;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "files", TrackTimestamps = true)]
public class FileSchema : BaseShareItemSchema
{
    public Guid ShareId { get; private set; }

    public FileMetadata Metadata { get; private set; }

    public string Url { get; private set; }

    public bool IsArchived { get; private set; }

    private FileSchema() { }

    public FileSchema(Guid shareId, FileMetadata metadata, string url, string? appVersion, AppSource? appSource)
    {
        Source = appSource;
        AppVersion = appVersion;

        Id = Guid.NewGuid();
        ShareId = shareId;
        Metadata = metadata;
        Url = url;
        IsArchived = false;
    }

    public void Archive() => IsArchived = true;
}

public record FileMetadata(
    string ExternalId,
    string Name,
    string Extension,
    long SizeInBytes,
    string MimeType,
    string? Checksum
);
