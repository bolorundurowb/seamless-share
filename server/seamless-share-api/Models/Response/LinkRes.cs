using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Models.Response;

public class LinkRes
{
    public string Id { get; set; } = null!;

    public string Url { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public AppSource? Source { get; protected set; }

    public string? DeviceName { get; protected set; }

    public string? AppVersion { get; protected set; }
}
