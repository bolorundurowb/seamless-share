using meerkat;

namespace SeamlessShareApi.Models.Data;

public abstract class BaseShareItemSchema : Schema<Guid>
{
    public AppSource? Source { get; protected set; }

    public string? DeviceName { get; protected set; }

    public string? AppVersion { get; protected set; }

    // a service offered to shared items with known owners. After 30 days, archived items will be deleted
    public bool IsArchived { get; protected set; }

    public DateTimeOffset? ArchivedAt { get; protected set; }

    public void Archive()
    {
        IsArchived = true;
        ArchivedAt = DateTimeOffset.UtcNow;
    }
}