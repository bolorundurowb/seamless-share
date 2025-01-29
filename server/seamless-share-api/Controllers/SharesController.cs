using Microsoft.AspNetCore.Mvc;
using SeamlessShareApi.Models.Data;
using SeamlessShareApi.Models.Request;
using SeamlessShareApi.Models.Response;
using SeamlessShareApi.Services;
using SeamlessShareApi.Utils;

namespace SeamlessShareApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SharesController(
    ILogger<SharesController> logger,
    ShareService shareService,
    LinkService linkService,
    TextService textService,
    AuthService authService,
    FileService fileService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOwnedShare()
    {
        var ownerId = authService.GetOwnerId(User);

        if (!ownerId.HasValue) 
            return BadRequest(new GenericMessage("Only authenticated users can access owned shares"));

        var share = await shareService.GetOwned(ownerId.Value);

        if (share is null)
            return NotFound(new GenericMessage("Share not found"));

        return Ok(share);
    }
    
    [HttpGet("{shareCode}")]
    public async Task<IActionResult> GetShareByCode(string shareCode)
    {
        var ownerId = authService.GetOwnerId(User);
        var share = await shareService.GetOneByCode(shareCode, ownerId);

        if (share is null)
            return NotFound(new GenericMessage("Share not found"));

        return Ok(share);
    }

    [HttpPost]
    public async Task<IActionResult> CreateShare()
    {
        var ownerId = authService.GetOwnerId(User);
        var requestInfo = RequestInfoExtractor.ExtractIpAddressAndUserAgent(HttpContext);
        var share = await shareService.Create(ownerId, requestInfo.IpAddress, requestInfo.UserAgent);

        return Ok(share);
    }

    [HttpPost("{shareId:guid}/text")]
    public async Task<IActionResult> AddTextShare(Guid shareId, [FromBody] AddTextToShareReq req)
    {
        var ownerId = authService.GetOwnerId(User);
        var share = await shareService.GetOne(shareId, ownerId);

        if (share is null)
            return NotFound(new GenericMessage("Share not found"));

        var isLink = TextCategorizer.IsLink(req.Content);
        BaseShareItemSchema? sharedContent;

        if (isLink)
            sharedContent = await linkService.Create(shareId, req.Content);
        else
            sharedContent = await textService.Create(shareId, req.Content);

        return Ok(sharedContent);
    }

    [HttpPost("{shareId:guid}/file")]
    public async Task<IActionResult> AddFileShare(Guid shareId, AddFileToShareReq req)
    {
        var ownerId = authService.GetOwnerId(User);

        if (!ownerId.HasValue)
            return BadRequest(new GenericMessage("Only authenticated users can upload files"));

        var share = await shareService.GetOne(shareId, ownerId.Value);

        if (share is null)
            return NotFound(new GenericMessage("Share not found"));

        var uploadResult = await fileService.Upload(ownerId.Value, req.Content);

        if (uploadResult is null)
            return NotFound(new GenericMessage("File upload failed"));

        var (fileUrl, fileMetadata) = uploadResult.Value;
        var file = await fileService.Create(ownerId.Value, fileUrl, fileMetadata);

        return Ok(file);
    }

    private async Task<(bool, IActionResult?)> CheckShareOwner(Guid shareId, Guid? ownerId)
    {
        var hasAccess = await shareService.HasShareAccess(shareId, ownerId);

        if (hasAccess)
            return (true, null);

        logger.LogWarning("An attempt was made to access a share that does not belong to the user. {ShareId} {UserId}",
            shareId, ownerId);
        return (false, NotFound("Share not found"));
    }
}
