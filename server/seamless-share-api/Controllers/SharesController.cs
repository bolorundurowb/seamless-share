using Microsoft.AspNetCore.Mvc;
using SeamlessShareApi.Services;
using SeamlessShareApi.Utils;

namespace SeamlessShareApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SharesController(ShareService shareService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateShare()
    {
        var requestInfo = RequestInfoExtractor.ExtractIpAddressAndUserAgent(HttpContext);
        var share = await shareService.Create(null, requestInfo.IpAddress, requestInfo.UserAgent);

        return Ok(share);
    }
}
