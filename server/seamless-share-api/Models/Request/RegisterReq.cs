namespace SeamlessShareApi.Models.Request;

public class RegisterReq
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string Password { get; set; } = null!;
}
