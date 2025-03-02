using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeamlessShareApi.Models.Request;
using SeamlessShareApi.Models.Response;
using SeamlessShareApi.Utils;

namespace SeamlessShareApi.Controllers;

public partial class SharesController
{
    [HttpGet("{shareId:guid}/documents")]
    [ProducesResponseType(typeof(List<FileRes>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSharedFiles(Guid shareId)
    {
        var ownerId = authService.GetOwnerId(User);
        var hasAccess = await shareService.HasShareAccess(shareId, ownerId);

        if (!hasAccess)
            return NotFound(new GenericMessage("Share not found"));

        var documents = await documentService.GetAll(shareId);

        return Ok(documents.Select(x => _fileMapper.MapDocument(x)).ToList());
    }

    [HttpPost("{shareId:guid}/document")]
    [ProducesResponseType(typeof(FileRes), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddDocumentShare(Guid shareId, AddDocumentToShareReq req)
    {
        var ownerId = authService.GetOwnerId(User);

        if (!ownerId.HasValue)
            return BadRequest(new GenericMessage("Only authenticated users can upload documents"));

        var share = await shareService.GetOne(shareId, ownerId.Value);

        if (share is null)
            return NotFound(new GenericMessage("Share not found"));

        var uploadResult = await fileService.Upload(share.Code, req.Content);

        if (uploadResult is null)
            return NotFound(new GenericMessage("Document upload failed"));

        var (documentUrl, fileMetadata) = uploadResult.Value;
        var (version, source) = RequestInfoExtractor.ExtractAppVersionAndSource(HttpContext);
        var document = await documentService.Create(ownerId.Value, documentUrl, fileMetadata, version, source);

        return Ok(document);
    }

    [Authorize]
    [HttpDelete("{shareId:guid}/documents/{documentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSharedDocument(Guid shareId, Guid documentId)
    {
        var ownerId = authService.GetOwnerId(User);

        if (!ownerId.HasValue)
            return BadRequest(new GenericMessage("Only authenticated users can access owned shares"));

        var hasAccess = await shareService.HasShareAccess(shareId, ownerId);

        if (!hasAccess)
            return NotFound(new GenericMessage("Share or document not found"));

        await documentService.ArchiveOne(shareId, documentId);

        return Ok();
    }
}