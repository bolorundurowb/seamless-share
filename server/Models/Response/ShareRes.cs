using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Models.Response;

public class ShareRes
{
    public string Id { get; set; }

    public string Code { get; set; }

    public ShareMetadata Metadata { get; set; }
}
