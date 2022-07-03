namespace Scaffold.WebApi.Controllers;

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

[ApiController]
[Route("demo")]
public class DemoController : ControllerBase
{
    private const string ExampleRequestRouteTemplate = "request";

    private readonly IClient demoClient;

    private readonly Options options;

    public DemoController(IClient demoClient, IOptions<Options> options)
    {
        this.demoClient = demoClient;
        this.options = options.Value;
    }

    public interface IClient
    {
        Task Trace(int depth, int fanOut, bool sync, CancellationToken cancellationToken = default);
    }

    /// <summary>Creates an example request.</summary>
    /// <param name="duration">The minimum duration in milliseconds the request should at least take.</param>
    /// <param name="statusCode">The HTTP status code the request should return.</param>
    /// <returns>A custom Problem Details (RFC 7807) response.</returns>
    /// <response code="default">Problem Details (RFC 7807).</response>
    [HttpDelete(ExampleRequestRouteTemplate)]
    [HttpGet(ExampleRequestRouteTemplate)]
    [HttpPost(ExampleRequestRouteTemplate)]
    [HttpPut(ExampleRequestRouteTemplate)]
    public async Task<ActionResult> ExampleRequest(int duration = 0, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        if (duration > 0)
        {
            if (duration > this.options.RequestMaxDuration)
            {
                duration = this.options.RequestMaxDuration;
            }

            await Task.Delay(duration);
        }

        return this.Problem(title: "Example Request", statusCode: (int)statusCode);
    }

    /// <summary>Creates an example trace.</summary>
    /// <param name="depth">The number of downstream services to call in the chain.</param>
    /// <param name="fanOut">The number of parallel calls to make to each downstream service.</param>
    /// <param name="sync">Whether to wait for downstream requests to complete before returning a response.</param>
    /// <returns>A custom Problem Details (RFC 7807) response.</returns>
    /// <response code="default">Problem Details (RFC 7807).</response>
    [HttpGet("trace")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Trace(int depth = 2, int fanOut = 1, bool sync = true)
    {
        if (depth > 0 && this.options.TraceMaxDepth > 0)
        {
            if (depth > this.options.TraceMaxDepth)
            {
                depth = this.options.TraceMaxDepth;
            }
            else
            {
                depth--;
            }

            if (fanOut > this.options.TraceMaxFanOut)
            {
                fanOut = this.options.TraceMaxFanOut;
            }

            if (fanOut < 1)
            {
                fanOut = 1;
            }

            List<Task> requests = new List<Task>();

            for (int i = 0; i < fanOut; i++)
            {
                requests.Add(this.demoClient.Trace(depth, fanOut, sync));
            }

            if (sync)
            {
                await Task.WhenAll(requests);
            }
        }

        return this.Problem(title: "Example Trace", statusCode: (int)HttpStatusCode.OK);
    }

    public class Client : IClient
    {
        private readonly HttpClient httpClient;

        private readonly Options options;

        public Client(HttpClient httpClient, IOptions<Options> options)
        {
            this.httpClient = httpClient;
            this.options = options.Value;
        }

        public async Task Trace(int depth, int fanOut, bool sync, CancellationToken cancellationToken = default)
        {
            Dictionary<string, string?> queryString = new Dictionary<string, string?>
            {
                [nameof(depth)] = depth.ToString(),
                [nameof(fanOut)] = fanOut.ToString(),
                [nameof(sync)] = sync.ToString(),
            };

            string uri = QueryHelpers.AddQueryString($"{this.options.NextHopBaseAddress}/demo/trace", queryString);
            await this.httpClient.GetAsync(uri, cancellationToken);
        }
    }

    public class Options
    {
        public string? NextHopBaseAddress { get; set; }

        public int RequestMaxDuration { get; set; } = 30000;

        public int TraceMaxDepth { get; set; } = 5;

        public int TraceMaxFanOut { get; set; } = 5;
    }
}
