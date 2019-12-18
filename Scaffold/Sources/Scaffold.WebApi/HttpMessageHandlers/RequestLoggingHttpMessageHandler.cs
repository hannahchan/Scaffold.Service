namespace Scaffold.WebApi.HttpMessageHandlers
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class RequestLoggingHttpMessageHandler : DelegatingHandler
    {
        private const string MessageTemplate = "Outbound HTTP {HttpMethod} {Uri} responded with {StatusCode} in {ElapsedMilliseconds}ms";

        private static readonly Action<ILogger, HttpMethod, Uri, int, long, Exception?> LogError =
            LoggerMessage.Define<HttpMethod, Uri, int, long>(LogLevel.Error, default, MessageTemplate);

        private static readonly Action<ILogger, HttpMethod, Uri, int, long, Exception?> LogInformation =
            LoggerMessage.Define<HttpMethod, Uri, int, long>(LogLevel.Information, default, MessageTemplate);

        private static readonly Action<ILogger, HttpMethod, Uri, int, long, Exception?> LogWarning =
            LoggerMessage.Define<HttpMethod, Uri, int, long>(LogLevel.Warning, default, MessageTemplate);

        private readonly ILogger logger;

        public RequestLoggingHttpMessageHandler(ILogger<RequestLoggingHttpMessageHandler> logger)
        {
            this.logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            int statusCode = (int)response.StatusCode;

            if (statusCode >= 200 && statusCode <= 299)
            {
                LogInformation(this.logger, request.Method, request.RequestUri, statusCode, stopwatch.ElapsedMilliseconds, null);
                return response;
            }

            if (statusCode >= 500)
            {
                LogError(this.logger, request.Method, request.RequestUri, statusCode, stopwatch.ElapsedMilliseconds, null);
                return response;
            }

            LogWarning(this.logger, request.Method, request.RequestUri, statusCode, stopwatch.ElapsedMilliseconds, null);

            return response;
        }
    }
}
