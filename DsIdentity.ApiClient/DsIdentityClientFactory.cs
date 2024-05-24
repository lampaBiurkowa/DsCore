using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace DsIdentity.ApiClient;

public class DsIdentityClientFactory(IHttpClientFactory httpClientFactory, IOptions<DsIdentityOptions> options)
{
    readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    readonly DsIdentityOptions options = options.Value;

    public DsIdentityClient CreateClient(string bearerToken)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new ($"{options.Url.TrimEnd('/')}/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        return new DsIdentityClient(client);
    }
}
