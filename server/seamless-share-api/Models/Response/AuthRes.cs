namespace SeamlessShareApi.Models.Response;

public record AuthRes(DateTime ExpiresAt, string AccessToken, UserRes User);
