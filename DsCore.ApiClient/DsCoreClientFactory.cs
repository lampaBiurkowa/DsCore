using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace DsCore.ApiClient;

public class DsCoreClientFactory(IHttpClientFactory httpClientFactory, IOptions<DsCoreOptions> options)
{
    readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    readonly DsCoreOptions options = options.Value;

    public DsCoreClient CreateClient(string bearerToken)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new ($"{options.Url.TrimEnd('/')}/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        return new DsCoreClient(client);
    }
}
