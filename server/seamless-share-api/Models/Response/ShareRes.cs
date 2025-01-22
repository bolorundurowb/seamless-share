using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Models.Response;

public class ShareRes
{
    public new Guid Id { get; set; }

    public string Code { get; set; }

    public ShareMetadata Metadata { get; set; }
}
