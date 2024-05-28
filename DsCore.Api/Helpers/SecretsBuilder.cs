using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DsCore.Api.Options;
using DsCore.ApiClient;

namespace DsCore.Api.Helpers;

public static class SecretsBuilder //TODO remove public
{
    public static string GenerateSalt(int size = 16)
    {
        var saltBytes = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    public static string CreatePasswordHash(string passwordBase64, string salt)
    {
        using var hmac = new HMACSHA512(Convert.FromBase64String(salt));
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(passwordBase64)));
    }

    public static string GenerateVerificationCode(int length = 6)
    {
        var random = new Random();
        var builder = new StringBuilder();
        for (int i = 0; i < length; i++)
            builder.Append(random.Next(10));

        return builder.ToString();
    }

    public static string TextToBase64(string text)
    {
        byte[] textBytes = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(textBytes);
    }

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