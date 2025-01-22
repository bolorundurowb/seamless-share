using meerkat;
using meerkat.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using shortid;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "shares", TrackTimestamps = true)]
public class ShareSchema : Schema
{
    public Guid? OwnerId { get; private set; }

    [Unique]
    public string Code { get; private set; }

    public ShareMetadata Metadata { get; private set; }

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
