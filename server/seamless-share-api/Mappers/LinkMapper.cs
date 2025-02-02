using Riok.Mapperly.Abstractions;
using SeamlessShareApi.Models.Data;
using SeamlessShareApi.Models.Response;

namespace SeamlessShareApi.Mappers;

[Mapper]
public partial class LinkMapper
{
    public partial LinkRes Map(LinkSchema linkDto);
}
