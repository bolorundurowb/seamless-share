using meerkat;
using meerkat.Attributes;

namespace SeamlessShareApi.Models.Data;

[Collection(Name = "users", TrackTimestamps = true)]
public class UserSchema : Schema<Guid>
{
    public string? FirstName { get; private set; }

    public string? LastName { get; private set; }

    public string EmailAddress { get; private set; }
    public string PasswordHash { get; private set; }

    public DateTimeOffset JoinedAt { get; private set; }

    public DateTimeOffset? LastLoginAt { get; private set; }

    private UserSchema() { }

    public UserSchema(string emailAddress, string password, string? firstName = null, string? lastName = null)
    {
        Id = Guid.NewGuid();
        JoinedAt = DateTimeOffset.UtcNow;

        EmailAddress = emailAddress.ToLowerInvariant().Trim();
        FirstName = firstName;
        LastName = lastName;

        PasswordHash = HashText(password);
    }

    public string Name() => $"{FirstName} {LastName}".Trim();

    public bool VerifyPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }

    public void SetLastLoginAt() => LastLoginAt = DateTimeOffset.UtcNow;

    private string HashText(string text)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        return BCrypt.Net.BCrypt.HashPassword(text, salt);
    }
}
