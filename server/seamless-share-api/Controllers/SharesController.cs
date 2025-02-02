using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeamlessShareApi.Mappers;
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
    private readonly ShareMapper _shareMapper = new();
    private readonly LinkMapper _linkMapper = new();
    private readonly FileMapper _fileMapper = new();
    private readonly TextMapper _textMapper = new();
    
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ShareSchema), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOwnedShare()
    {
        var ownerId = authService.GetOwnerId(User);

        if (!ownerId.HasValue)
            return BadRequest(new GenericMessage("Only authenticated users can access owned shares"));

        var share = await shareService.GetOwned(ownerId.Value);

        if (share is null)
            return NotFound(new GenericMessage("Share not found"));

        return Ok(_shareMapper.Map(share));
    }

    [HttpGet("{shareCode}")]
    [ProducesResponseType(typeof(ShareSchema), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShareByCode(string shareCode)
    {
        var ownerId = authService.GetOwnerId(User);
        var share = await shareService.GetOneByCode(shareCode, ownerId);

        if (share is null)
            return NotFound(new GenericMessage("Share not found"));

        return Ok(share);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ShareSchema), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateShare()
    {
        var ownerId = authService.GetOwnerId(User);
        var requestInfo = RequestInfoExtractor.ExtractIpAddressAndUserAgent(HttpContext);
        var share = await shareService.Create(ownerId, requestInfo.IpAddress, requestInfo.UserAgent);

        return Ok(share);
    }

    [HttpGet("{shareId:guid}/links")]
    [ProducesResponseType(typeof(List<LinkRes>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSharedLinks(Guid shareId)
    {
        var ownerId = authService.GetOwnerId(User);
        var hasAccess = await shareService.HasShareAccess(shareId, ownerId);

        if (!hasAccess)
            return NotFound(new GenericMessage("Share not found"));

        var links = await linkService.GetAll(shareId);
        
        return Ok(links.Select(x => _linkMapper.Map(x)).ToList());
    }

    [HttpGet("{shareId:guid}/files")]
    [ProducesResponseType(typeof(List<FileRes>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSharedFiles(Guid shareId)
    {
        var ownerId = authService.GetOwnerId(User);
        var hasAccess = await shareService.HasShareAccess(shareId, ownerId);

        if (!hasAccess)
            return NotFound(new GenericMessage("Share not found"));

        var files = await fileService.GetAll(shareId);
        
        return Ok(files.Select(x => _fileMapper.Map(x)).ToList());
    }

    [HttpPost("{shareId:guid}/text")]
    [ProducesResponseType(typeof(BaseShareItemSchema), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(FileSchema), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddFileShare(Guid shareId, AddFileToShareReq req)
    {
        var ownerId = authService.GetOwnerId(User);

        if (!ownerId.HasValue)
            return BadRequest(new GenericMessage("Only authenticated users can upload files"));

        var share = await shareService.GetOne(shareId, ownerId.Value);

        if (share is null)
            return NotFound(new GenericMessage("Share not found"));

        var uploadResult = await fileService.Upload(share.Code, req.Content);

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