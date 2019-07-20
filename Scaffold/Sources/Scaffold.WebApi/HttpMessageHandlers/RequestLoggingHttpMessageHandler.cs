namespace Scaffold.WebApi.HttpMessageHandlers
{
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class RequestLoggingHttpMessageHandler : DelegatingHandler
    {
        private const string MessageTemplate = "Outbound HTTP {HttpMethod} {Uri} responded with {StatusCode} in {ElapsedMilliseconds}ms";

        private readonly ILogger logger;

        public RequestLoggingHttpMessageHandler(ILogger<RequestLoggingHttpMessageHandler> logger) => this.logger = logger;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            int statusCode = (int)response.StatusCode;
            LogLevel logLevel = LogLevel.Information;

            if (statusCode < 200 || statusCode > 299)
            {
                logLevel = LogLevel.Warning;
            }

            if (statusCode >= 500)
            {
                logLevel = LogLevel.Error;
            }

            this.logger.Log(
                logLevel,
                MessageTemplate,
                request.Method,
                request.RequestUri,
                statusCode,
                stopwatch.ElapsedMilliseconds);

            return response;
        }
    }
}
