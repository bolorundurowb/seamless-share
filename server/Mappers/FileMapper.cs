using Riok.Mapperly.Abstractions;
using SeamlessShareApi.Models.Data;
using SeamlessShareApi.Models.Response;
using SeamlessShareApi.Utils;

namespace SeamlessShareApi.Mappers;

[Mapper]
public partial class FileMapper
{
    public partial FileRes MapDocument(DocumentSchema documentDto);

    public partial FileRes MapImage(ImageSchema imageDto);

    public FileRes MapDocumentToDto(DocumentSchema documentDto, bool isAuthenticated)
    {
        var dto = MapDocument(documentDto);
        dto.ExpiresAt = GenericUtils.GetExpirationDate(documentDto.CreatedAt, isAuthenticated);

        return dto;
    }

    public FileRes MapImageToDto(ImageSchema imageDto, bool isAuthenticated)
    {
        var dto = MapImage(imageDto);
        dto.ExpiresAt = GenericUtils.GetExpirationDate(imageDto.CreatedAt, isAuthenticated);

        return dto;
    }
}
