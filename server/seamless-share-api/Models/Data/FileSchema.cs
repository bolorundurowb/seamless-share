using meerkat.Attributes;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "files", TrackTimestamps = true)]
public class FileSchema : BaseShareItemSchema
{
    public new Guid Id { get; private set; }

    public Guid ShareId { get; private set; }

    public FileMetadata Metadata { get; private set; }

    public string Url { get; private set; }

    public bool IsArchived { get; private set; }
}

public record FileMetadata(
    string Id,
    string Name,
    string Extension,
    long SizeInBytes,
    string MimeType,
    string Checksum
);
