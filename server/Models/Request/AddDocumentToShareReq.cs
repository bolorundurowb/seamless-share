namespace SeamlessShareApi.Models.Request;

public class AddDocumentToShareReq
{
    public IFormFile Content { get; set; } = null!;
}
