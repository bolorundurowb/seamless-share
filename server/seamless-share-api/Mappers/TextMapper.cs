using Riok.Mapperly.Abstractions;
using SeamlessShareApi.Models.Data;
using SeamlessShareApi.Models.Response;

namespace SeamlessShareApi.Mappers;

[Mapper]
public partial class TextMapper
{
    public partial TextRes Map(TextSchema textDto);
}
