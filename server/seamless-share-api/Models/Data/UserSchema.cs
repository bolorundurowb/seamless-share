using meerkat;
using meerkat.Attributes;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "users", TrackTimestamps = true)]
public class UserSchema : Schema
{
    public string? FirstName { get; private set; }

    public string? LastName { get; private set; }

    public string Name() => $"{FirstName} {LastName}".Trim();
}
