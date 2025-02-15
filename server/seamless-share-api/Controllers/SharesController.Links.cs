using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeamlessShareApi.Models.Response;

namespace SeamlessShareApi.Controllers;

public partial class SharesController
{
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
    
    [Authorize]
    [HttpDelete("{shareId:guid}/links/{linkId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteASharedLink(Guid shareId, Guid linkId)
    {
        var ownerId = authService.GetOwnerId(User);

        if (!ownerId.HasValue)
            return BadRequest(new GenericMessage("Only authenticated users can access owned shares"));
        
        var hasAccess = await shareService.HasShareAccess(shareId, ownerId);

        if (!hasAccess)
            return NotFound(new GenericMessage("Share or link not found"));

        await linkService.DeleteOne(shareId, linkId);

        return Ok();
    }
}
