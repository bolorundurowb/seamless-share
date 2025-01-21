using meerkat;
using meerkat.Attributes;
using shortid;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "shares", TrackTimestamps = true)]
public class ShareSchema : Schema
{
    public new Guid Id { get; private set; }

    public Guid? OwnerId { get; private set; }

    [Unique]
    public string Code { get; private set; }

    public ShareMetadata Metadata { get; set; }

    private ShareSchema() { }

    public ShareSchema(Guid? ownerId = null, string? ipAddress = null, string? userAgent = null)
    {
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        Code = ShortId.Generate();
        Metadata = new ShareMetadata(ipAddress, userAgent);
    }
}

public record ShareMetadata(string? IpAddress, string? UserAgent);
