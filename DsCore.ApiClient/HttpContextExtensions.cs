using Microsoft.AspNetCore.Http;

namespace DsCore.ApiClient;

public static class HttpContextExtensions
{
    public static Guid? GetUserGuid(this HttpContext ctx)
    {
        var claim = ctx.User.Claims.FirstOrDefault(x => x.Type == ClaimNames.USER_GUID)?.Value;
        if (Guid.TryParse(claim, out var result))
            return result;
        
        return null;
    }

    public static string? GetBearerToken(this HttpContext ctx)
    {
        var authorizationHeader = ctx.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return authorizationHeader["Bearer ".Length..].Trim();

        return null;
    }

    public static bool IsUser(this HttpContext ctx, Guid userGuid) =>
        ctx.User.Claims.FirstOrDefault(x => x.Type == ClaimNames.USER_GUID)?.Value == userGuid.ToString();
}
