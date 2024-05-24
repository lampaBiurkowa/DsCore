namespace DsIdentity.Api.Options;

public class TokenOptions
{
    public const string SECTION = "Token";

    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
    public required int ExpiresInMinutes { get; set; } = 5;
}