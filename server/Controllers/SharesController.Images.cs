using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeamlessShareApi.Models.Request;
using SeamlessShareApi.Models.Response;
using SeamlessShareApi.Utils;

namespace SeamlessShareApi.Controllers;

public partial class SharesController
{
    [HttpGet("{shareId:guid}/images")]
    [ProducesResponseType(typeof(List<FileRes>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSharedImages(Guid shareId)
    {
        var ownerId = authService.GetOwnerId(User);
        var hasAccess = await shareService.HasShareAccess(shareId, ownerId);

        if (!hasAccess)
            return NotFound(new GenericMessage("Share not found"));

        var images = await imageService.GetAll(shareId);
        var mappedImages = new List<FileRes>();

        if (images.Count > 0)
            foreach (var image in images)
            {
                var mappedImage = _fileMapper.MapImageToDto(image, ownerId.HasValue);
                mappedImage.Url = fileService.GetSignedUrl(image.Url);
                mappedImages.Add(mappedImage);
            }

        return Ok(mappedImages);
    }

    [HttpPost("{shareId:guid}/images")]
    [ProducesResponseType(typeof(FileRes), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddImageShare(Guid shareId, AddImageToShareReq req)
    {
        var ownerId = authService.GetOwnerId(User);

        if (!ownerId.HasValue)
            return BadRequest(new GenericMessage("Only authenticated users can upload documents"));

        var share = await shareService.GetOne(shareId, ownerId.Value);

        if (share is null)
            return NotFound(new GenericMessage("Share not found"));

        var uploadResult = await fileService.Upload(share.Code, Constants.ImagesFolderName, req.Content);

        if (uploadResult is null)
            return NotFound(new GenericMessage("Image upload failed"));

        var (imageUrl, fileMetadata) = uploadResult.Value;
        var (version, source) = RequestInfoExtractor.ExtractAppVersionAndSource(HttpContext);
        var image = await imageService.Create(shareId, imageUrl, fileMetadata, version, source);
        
        var mappedImage = _fileMapper.MapImage(image);
        mappedImage.Url = fileService.GetSignedUrl(image.Url);

        return Ok(mappedImage);
    }

    [Authorize]
    [HttpDelete("{shareId:guid}/images/{imageId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericMessage), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSharedImage(Guid shareId, Guid imageId)
    {
        var ownerId = authService.GetOwnerId(User);

        if (!ownerId.HasValue)
            return BadRequest(new GenericMessage("Only authenticated users can access owned shares"));

        var hasAccess = await shareService.HasShareAccess(shareId, ownerId);

        if (!hasAccess)
            return NotFound(new GenericMessage("Share or image not found"));

        await imageService.ArchiveOne(shareId, imageId);

        return Ok();
    }
}
