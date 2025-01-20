using meerkat;
using meerkat.Attributes;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "shares", TrackTimestamps = true)]
public class Share : Schema
{
    public new Guid Id { get;  private set; }
    
    public Guid? OwnerId { get;  private set; }

    public string Code { get; private set; }
}
