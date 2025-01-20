using meerkat;
using meerkat.Attributes;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "shares", TrackTimestamps = true)]
public class ShareSchema : Schema
{
    public new Guid Id { get; private set; }

    public Guid? OwnerId { get; private set; }

    [Unique]
    public string Code { get; private set; }

    public ShareMetadata Metadata { get; set; }
}

public record ShareMetadata(string? IpAddress, string? UserAgent);
