using meerkat.Attributes;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "texts", TrackTimestamps = true)]
public class TextSchema : BaseShareItemSchema
{
    public new Guid Id { get; private set; }

    public Guid ShareId { get; private set; }

    public string Content { get; set; }

    private TextSchema() { }

    public TextSchema(Guid shareId, string content)
    {
        Id = Guid.NewGuid();
        ShareId = shareId;
        Content = content;
    }
}
