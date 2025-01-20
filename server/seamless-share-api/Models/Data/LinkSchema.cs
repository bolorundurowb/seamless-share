using meerkat.Attributes;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "links", TrackTimestamps = true)]
public class LinkSchema : BaseShareItemSchema
{
    public new Guid Id { get; private set; }

    public Guid ShareId { get; private set; }

    public string Url { get; private set; }
}
