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
    public async Task<IActionResult> GetSharedDocuments(Guid shareId)
    {
        var ownerId = authService.GetOwnerId(User);
        var hasAccess = await shareService.HasShareAccess(shareId, ownerId);

        if (!hasAccess)
            return NotFound(new GenericMessage("Share not found"));

        var documents = await documentService.GetAll(shareId);
        var mappedDocuments = new List<FileRes>();

        if (documents.Count != 0)
            foreach (var document in documents)
            {
                var mappedDocument = _fileMapper.MapDocument(document);
                mappedDocument.Url = fileService.GetSignedUrl(document.Url);
                mappedDocuments.Add(mappedDocument);
            }

        return Ok(mappedDocuments);
    }

    [HttpPost("{shareId:guid}/documents")]
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

        var uploadResult = await fileService.Upload(share.Code, Constants.DocumentsFolderName, req.Content);

        if (uploadResult is null)
            return NotFound(new GenericMessage("Document upload failed"));

        var (documentUrl, fileMetadata) = uploadResult.Value;
        var (version, source) = RequestInfoExtractor.ExtractAppVersionAndSource(HttpContext);
        var document = await documentService.Create(shareId, documentUrl, fileMetadata, version, source);
        
        var mappedDocument = _fileMapper.MapDocument(document);
        mappedDocument.Url = fileService.GetSignedUrl(document.Url);

        return Ok(mappedDocument);
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
