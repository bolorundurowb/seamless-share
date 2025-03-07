using meerkat;
using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Services;

public class UserService(ILogger<UserService> logger)
{
    public Task<UserSchema?> FindByEmail(string emailAddress)
    {
        logger.LogDebug("Getting user {EmailAddress}", emailAddress);
        return Meerkat.FindOneAsync<UserSchema, Guid>(x => x.EmailAddress == emailAddress.ToLowerInvariant());
    }

    public async Task<UserSchema> Create(string emailAddress, string password, string? firstName = null,
        string? lastName = null)
    {
        logger.LogDebug("Creating a new user. {EmailAddress} {FirstName} {LastName}", emailAddress, firstName,
            lastName);

        var user = new UserSchema(emailAddress, password, firstName, lastName);
        await user.SaveAsync();

        logger.LogDebug("User successfully created. {UserId} {EmailAddress} {FirstName} {LastName}", user.Id,
            emailAddress, firstName, lastName);

        return user;
    }
}