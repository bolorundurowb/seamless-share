namespace SeamlessShareApi.Models.Request;

public class AddFileToShareReq
{
    public IFormFile Content { get; set; } = null!;
}
