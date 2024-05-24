namespace DsIdentity.ApiClient;

public class DsIdentityOptions
{
    public const string SECTION = nameof(DsIdentity);
    public required string Url { get; set; }
    public required string SecretKey { get; set; }
}