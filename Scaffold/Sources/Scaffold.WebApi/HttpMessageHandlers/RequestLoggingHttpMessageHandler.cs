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
        private const string RequestStartedMessageTemplate = "Outbound HTTP {HttpMethod} {Uri} started";

        private const string RequestFinishedMessageTemplate = "Outbound HTTP {HttpMethod} {Uri} finished in {ElapsedMilliseconds}ms - {StatusCode}";

        private static readonly Action<ILogger, HttpMethod, Uri, Exception?> LogRequestStarted =
            LoggerMessage.Define<HttpMethod, Uri>(LogLevel.Information, default, RequestStartedMessageTemplate);

        private static readonly Action<ILogger, HttpMethod, Uri, long, int, Exception?> LogInformation =
            LoggerMessage.Define<HttpMethod, Uri, long, int>(LogLevel.Information, default, RequestFinishedMessageTemplate);

        private static readonly Action<ILogger, HttpMethod, Uri, long, int, Exception?> LogWarning =
            LoggerMessage.Define<HttpMethod, Uri, long, int>(LogLevel.Warning, default, RequestFinishedMessageTemplate);

        private static readonly Action<ILogger, HttpMethod, Uri, long, int, Exception?> LogError =
            LoggerMessage.Define<HttpMethod, Uri, long, int>(LogLevel.Error, default, RequestFinishedMessageTemplate);

        private readonly ILogger logger;

        public RequestLoggingHttpMessageHandler(ILogger<RequestLoggingHttpMessageHandler> logger)
        {
            this.logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LogRequestStarted(this.logger, request.Method, request.RequestUri, null);

            Stopwatch stopwatch = Stopwatch.StartNew();
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            int statusCode = (int)response.StatusCode;

            if (statusCode >= 200 && statusCode <= 299)
            {
                LogInformation(this.logger, request.Method, request.RequestUri, stopwatch.ElapsedMilliseconds, statusCode, null);
                return response;
            }

            if (statusCode >= 500)
            {
                LogError(this.logger, request.Method, request.RequestUri, stopwatch.ElapsedMilliseconds, statusCode, null);
                return response;
            }

            LogWarning(this.logger, request.Method, request.RequestUri, stopwatch.ElapsedMilliseconds, statusCode, null);

            return response;
        }
    }
}
