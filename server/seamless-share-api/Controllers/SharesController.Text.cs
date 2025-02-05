using Microsoft.AspNetCore.Mvc;
using SeamlessShareApi.Models.Data;
using SeamlessShareApi.Models.Request;
using SeamlessShareApi.Models.Response;
using SeamlessShareApi.Utils;

namespace SeamlessShareApi.Controllers;

public partial class SharesController
{
    [HttpGet("{shareId:guid}/text")]
    [ProducesResponseType(typeof(List<TextRes>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSharesText(Guid shareId)
    {
        var ownerId = authService.GetOwnerId(User);
        var hasAccess = await shareService.HasShareAccess(shareId, ownerId);

        if (!hasAccess)
            return NotFound(new GenericMessage("Share not found"));

        var text = await textService.GetAll(shareId);

        return Ok(text.Select(x => _textMapper.Map(x)).ToList());
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
        var (version, source) = RequestInfoExtractor.ExtractAppVersionAndSource(HttpContext);
        BaseShareItemSchema? sharedContent;

        if (isLink)
            sharedContent = await linkService.Create(shareId, req.Content, version, source);
        else
            sharedContent = await textService.Create(shareId, req.Content, version, source);

        return Ok(sharedContent);
    }
}
