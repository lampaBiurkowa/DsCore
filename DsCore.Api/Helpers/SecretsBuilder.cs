using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DsCore.Api.Options;
using DsCore.ApiClient;

namespace DsCore.Api.Helpers;

public static class JwtBuilder
{
    public static string BuildToken(Guid userId, TokenOptions options)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var claims = new[]
        {
            new Claim(ClaimNames.USER_GUID, userId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(options.ExpiresInMinutes)),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}