namespace SeamlessShareApi.Models.Request;

public class AddImageToShareReq
{
    public IFormFile Content { get; set; } = null!;
}
