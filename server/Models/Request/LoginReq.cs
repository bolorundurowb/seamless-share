namespace SeamlessShareApi.Models.Request;

public class LoginReq
{
    public string EmailAddress { get; set; } = null!;

    public string Password { get; set; } = null!;
}
