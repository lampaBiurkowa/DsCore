namespace DsCore.ApiClient;

public class DsCoreOptions
{
    public const string SECTION = nameof(DsCore);
    public required string Url { get; set; }
    public required string SecretKey { get; set; }
}