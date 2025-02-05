using Microsoft.AspNetCore.Mvc;
using SeamlessShareApi.Models.Data;
using SeamlessShareApi.Models.Request;
using SeamlessShareApi.Models.Response;

namespace SeamlessShareApi.Controllers;

public partial class SharesController
{
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
}
