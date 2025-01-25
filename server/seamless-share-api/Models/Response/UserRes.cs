namespace SeamlessShareApi.Models.Response;

public class UserRes
{
    public string Id { get;  set; }
    
    public string? FirstName { get;  set; }

    public string? LastName { get;  set; }

    public string EmailAddress { get;  set; }
    
    public DateTimeOffset JoinedAt { get;  set; }
}
