using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeamlessShareApi.Background;
using SeamlessShareApi.Mappers;
using SeamlessShareApi.Models.Data;
using SeamlessShareApi.Models.Response;
using SeamlessShareApi.Services;
using SeamlessShareApi.Utils;

namespace SeamlessShareApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public partial class SharesController(
    ILogger<SharesController> logger,
    ShareService shareService,
    LinkService linkService,
    TextService textService,
    AuthService authService,
    FileService fileService,
    DocumentService documentService,
    ImageService imageService,
    UrlMetadataService metadataService) : ControllerBase
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