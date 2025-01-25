using Riok.Mapperly.Abstractions;
using SeamlessShareApi.Models.Data;
using SeamlessShareApi.Models.Response;

namespace SeamlessShareApi.Mappers;

[Mapper]
public partial class UserMapper
{
    public partial UserRes MapToUserRes(UserSchema userDto);
}
