using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Models.Response;

public class FileRes
{
    public string Id { get; set; } = null!;

    public FileMetadata Metadata { get; set; }

    public string Url { get; set; } = null!;

    public bool IsArchived { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public AppSource? Source { get; set; }

    public string? DeviceName { get; set; }

    public string? AppVersion { get; set; }
    
    public DateTime ExpiresAt { get; set; }
}
