namespace Scaffold.WebApi.Controllers;

using System.Diagnostics;
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

    /// <summary>Creates an example chain of requests.</summary>
    /// <returns>A string containing the trace identifier (TraceID) for the chain.</returns>
    /// <response code="200">The chain of requests was created successfully.</response>
    /// <response code="default">Problem Details (RFC 7807) Response.</response>
    [HttpGet("trace")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<string?> Trace()
    {
        return await this.Trace(2);
    }

    /// <summary>Creates an example chain of requests.</summary>
    /// <param name="depth">The number of requests to make in the chain.</param>
    /// <returns>A string containing the trace identifier (TraceID) for the chain.</returns>
    /// <response code="200">The chain of requests was created successfully.</response>
    /// <response code="default">Problem Details (RFC 7807) Response.</response>
    [HttpGet("trace/{depth}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<string?> Trace(int depth)
    {
        if (depth > 0)
        {
            await this.demoClient.Trace(depth > this.options.MaxDepth ? this.options.MaxDepth : depth - 1);
        }

        return Activity.Current?.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => Activity.Current.RootId,
            ActivityIdFormat.W3C => Activity.Current.TraceId.ToHexString(),
            _ => null,
        };
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
            await this.httpClient.GetAsync($"{this.options.NextHopBaseAddress}/demo/trace/{depth}", cancellationToken);
        }
    }

    public class Options
    {
        public string? NextHopBaseAddress { get; set; }

        public int MaxDepth { get; set; } = 10;
    }
}
