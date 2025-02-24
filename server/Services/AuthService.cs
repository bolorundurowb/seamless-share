using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SeamlessShareApi.Models.Data;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace SeamlessShareApi.Services;

public class AuthService(ILogger<AuthService> logger, IConfiguration configuration)
{
    public (DateTime, string) GenerateJwtToken(UserSchema user)
    {
        logger.LogDebug("Generating JWT token for user {UserId} {UserName}", user.Id, user.Name());

        var jwtSettings = configuration.GetSection("JwtSettings");
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Secret"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Name()),
            new Claim(JwtRegisteredClaimNames.Sub, ((Guid)user.Id).ToString("D")),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiry = DateTime.Now.AddHours(12);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: credentials);

        return (expiry, new JwtSecurityTokenHandler().WriteToken(token));
    }

    public Guid? GetOwnerId(ClaimsPrincipal? claimsPrincipal)
    {
        if (claimsPrincipal == null)
            return null;

        // Try a few common claim types for "sub"
        var extractedHeaderValue = claimsPrincipal.FindFirstValue("sub")
                                   ?? claimsPrincipal.FindFirstValue(
                                       "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        return Guid.TryParse(extractedHeaderValue, out var ownerId) ? ownerId : null;
    }
}
