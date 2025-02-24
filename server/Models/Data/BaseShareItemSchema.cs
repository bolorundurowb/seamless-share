using meerkat;

namespace SeamlessShareApi.Models.Data;

public abstract class BaseShareItemSchema : Schema
{
    public AppSource? Source { get; protected set; }

    public string? DeviceName { get; protected set; }

    public string? AppVersion { get; protected set; }
}
