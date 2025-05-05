using Riok.Mapperly.Abstractions;
using SeamlessShareApi.Models.Data;
using SeamlessShareApi.Models.Response;
using SeamlessShareApi.Utils;

namespace SeamlessShareApi.Mappers;

[Mapper]
public partial class TextMapper
{
    public partial TextRes Map(TextSchema textDto);

    public TextRes MapToDto(TextSchema textDto, bool isAuthenticated)
    {
        var dto = Map(textDto);
        dto.ExpiresAt = GenericUtils.GetExpirationDate(textDto.CreatedAt, isAuthenticated);

        return dto;
    }
}
