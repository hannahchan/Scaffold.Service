namespace Scaffold.WebApi.Controllers;

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        Task Trace(int depth, CancellationToken cancellationToken = default);
    }

    /// <summary>Creates an example request.</summary>
    /// <param name="duration">The minimum duration in milliseconds the request should at least take.</param>
    /// <param name="statusCode">The HTTP status code the request should return.</param>
    /// <returns>Problem Details (RFC 7807) Response.</returns>
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

    /// <summary>Creates an example chain of requests.</summary>
    /// <param name="depth">The number of requests to make in the chain.</param>
    /// <returns>A string containing the trace identifier (TraceID) for the chain.</returns>
    /// <response code="default">Problem Details (RFC 7807) Response.</response>
    [HttpGet("trace")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Trace(int depth = 2)
    {
        if (depth > 0)
        {
            await this.demoClient.Trace(depth > this.options.TraceMaxDepth ? this.options.TraceMaxDepth : depth - 1);
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

        public async Task Trace(int depth, CancellationToken cancellationToken = default)
        {
            await this.httpClient.GetAsync($"{this.options.NextHopBaseAddress}/demo/trace?depth={depth}", cancellationToken);
        }
    }

    public class Options
    {
        public string? NextHopBaseAddress { get; set; }

        public int RequestMaxDuration { get; set; } = 30000;

        public int TraceMaxDepth { get; set; } = 10;
    }
}
