using meerkat.Attributes;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "links", TrackTimestamps = true)]
public class LinkSchema : BaseShareItemSchema
{
    public Guid ShareId { get; private set; }

    public string Url { get; private set; }

    private LinkSchema() { }

    public LinkSchema(Guid shareId, string url, string? appVersion, AppSource? appSource)
    {
        Source = appSource;
        AppVersion = appVersion;
        
        Id = Guid.NewGuid();
        ShareId = shareId;
        Url = url;
    }
}
