using Microsoft.AspNetCore.Mvc;
using SeamlessShareApi.Models.Request;
using SeamlessShareApi.Models.Response;
using SeamlessShareApi.Services;
using SeamlessShareApi.Utils;

namespace SeamlessShareApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SharesController(ShareService shareService, LinkService linkService) : ControllerBase
{
    [HttpGet("{shareId:guid}")]
    public async Task<IActionResult> GetShare(Guid sharedId)
    {
        var share = await shareService.GetOne(sharedId, null);

        if (share is null) 
            return NotFound(new GenericMessage("Share not found"));

        return Ok(share);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateShare()
    {
        var requestInfo = RequestInfoExtractor.ExtractIpAddressAndUserAgent(HttpContext);
        var share = await shareService.Create(null, requestInfo.IpAddress, requestInfo.UserAgent);

        return Ok(share);
    }
    
    [HttpPost("{shareId:guid}/text")]
    public async Task<IActionResult> AddTextShare(Guid shareId, [FromBody] AddToShareReq req)
    {
        var share = await shareService.GetOne(sharedId, null);

        if (share is null) 
            return NotFound(new GenericMessage("Share not found"));

        var isLink = TextCategorizer.IsLink(req.Content);

        if (isLink)
        {
          var link =  await linkService.Create(shareId, req.Content);
          return Ok(link);
        }
        else
        {
            
        }
    }
}
