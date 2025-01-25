using Microsoft.AspNetCore.Mvc;
using SeamlessShareApi.Mappers;
using SeamlessShareApi.Models.Data;
using SeamlessShareApi.Models.Request;
using SeamlessShareApi.Models.Response;
using SeamlessShareApi.Services;
using SeamlessShareApi.Utils;

namespace SeamlessShareApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(
    ILogger<AuthController> logger,
    UserService userService,
    AuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginReq req)
    {
        var user = await userService.FindByEmail(req.EmailAddress);

        if (user is null)
        {
            logger.LogWarning("An attempt was made to login with an invalid email address. {EmailAddress}",
                req.EmailAddress);
            return BadRequest(new GenericMessage("Check your credentials and try again."));
        }

        if (!user.VerifyPassword(req.Password))
        {
            logger.LogWarning("An attempt was made to login with an invalid password. {EmailAddress}",
                req.EmailAddress);
            return BadRequest(new GenericMessage("Check your credentials and try again."));
        }

        var res = await FinishLogin(user);
        return Ok(res);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterReq req)
    {
        var user = await userService.FindByEmail(req.EmailAddress);

        if (user is not null)
        {
            logger.LogWarning("An attempt was made to register with an email address already in use. {EmailAddress}",
                req.EmailAddress);
            return BadRequest(new GenericMessage("Account already exists."));
        }

        user = await userService.Create(req.EmailAddress, req.Password, req.FirstName, req.LastName);
        var res = await FinishLogin(user);
        return Ok(res);
    }

    private async Task<AuthRes> FinishLogin(UserSchema user)
    {
        var (expiresAt, accessToken) = authService.GenerateJwtToken(user);

        user.SetLastLoginAt();
        await user.SaveAsync();

        return new AuthRes(expiresAt, accessToken, new UserMapper().MapToUserRes(user));
    }
}
