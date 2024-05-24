using Microsoft.AspNetCore.Http;

namespace DsIdentity.ApiClient;

public static class HttpContextExtensions
{
    public static string? GetUserGuid(this HttpContext ctx) =>
        ctx.User.Claims.FirstOrDefault(x => x.Type == ClaimNames.USER_GUID)?.Value;

    public static string? GetBearerToken(this HttpContext ctx)
    {
        var authorizationHeader = ctx.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return authorizationHeader["Bearer ".Length..].Trim();

        return null;
    }
}
