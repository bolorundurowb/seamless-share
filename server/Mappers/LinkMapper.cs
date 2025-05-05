using Riok.Mapperly.Abstractions;
using SeamlessShareApi.Models.Data;
using SeamlessShareApi.Models.Response;
using SeamlessShareApi.Utils;

namespace SeamlessShareApi.Mappers;

[Mapper]
public partial class LinkMapper
{
    public partial LinkRes Map(LinkSchema linkDto);

    public LinkRes MapToDto(LinkSchema linkDto, bool isAuthenticated)
    {
        var dto = Map(linkDto);
        dto.ExpiresAt = GenericUtils.GetExpirationDate(linkDto.CreatedAt, isAuthenticated);

        return dto;
    }
}
