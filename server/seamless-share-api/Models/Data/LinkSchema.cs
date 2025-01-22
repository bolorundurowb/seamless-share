using meerkat.Attributes;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "links", TrackTimestamps = true)]
public class LinkSchema : BaseShareItemSchema
{
    public Guid ShareId { get; private set; }

    public string Url { get; private set; }

    private LinkSchema() { }

    public LinkSchema(Guid shareId, string url)
    {
        Id = Guid.NewGuid();
        ShareId = shareId;
        Url = url;
    }
}
