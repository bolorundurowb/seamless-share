﻿using meerkat;
using meerkat.Attributes;
using shortid;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "shares", TrackTimestamps = true)]
public class ShareSchema : Schema<Guid>
{
    public Guid? OwnerId { get; private set; }

    [UniqueIndex]
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
