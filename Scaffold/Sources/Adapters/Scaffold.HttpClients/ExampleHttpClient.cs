namespace Scaffold.HttpClients;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Scaffold.Application.Common.Interfaces;

public class ExampleHttpClient : IExampleHttpClient
{
    private const string BaseAddress = "https://worldtimeapi.org";

    private readonly HttpClient httpClient;

    public ExampleHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(BaseAddress);
        this.httpClient = httpClient;
    }

    public Task<HttpResponseMessage> Get(string path, CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(path ?? string.Empty, UriKind.Relative));
        return this.httpClient.SendAsync(httpRequestMessage, cancellationToken);
    }
}
