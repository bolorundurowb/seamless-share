using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Models.Response;

public class TextRes
{
    public string Id { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public AppSource? Source { get; set; }

    public string? DeviceName { get; set; }

    public string? AppVersion { get; set; }
    
    public DateTime ExpiresAt { get; set; }
}
