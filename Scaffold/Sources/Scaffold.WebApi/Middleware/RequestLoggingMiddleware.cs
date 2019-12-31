namespace Scaffold.WebApi.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class RequestLoggingMiddleware
    {
        private const string RequestStartedMessageTemplate = "Inbound HTTP {HttpMethod} {Path} started";

        private const string RequestFinishedMessageTemplate = "Inbound HTTP {HttpMethod} {Path} finished in {ElapsedMilliseconds}ms - {StatusCode}";

        private static readonly Action<ILogger, string, PathString, Exception?> LogRequestStarted =
            LoggerMessage.Define<string, PathString>(LogLevel.Information, default, RequestStartedMessageTemplate);

        private static readonly Action<ILogger, string, PathString, long, int, Exception?> LogRequestFinishedInformation =
            LoggerMessage.Define<string, PathString, long, int>(LogLevel.Information, default, RequestFinishedMessageTemplate);

        private static readonly Action<ILogger, string, PathString, long, int, Exception?> LogRequestFinishedWarning =
            LoggerMessage.Define<string, PathString, long, int>(LogLevel.Warning, default, RequestFinishedMessageTemplate);

        private static readonly Action<ILogger, string, PathString, long, int, Exception?> LogRequestFinishedError =
            LoggerMessage.Define<string, PathString, long, int>(LogLevel.Error, default, RequestFinishedMessageTemplate);

        private static readonly Action<ILogger, string, PathString, long, string, Exception?> LogRequestFinishedCritical =
            LoggerMessage.Define<string, PathString, long, string>(LogLevel.Critical, default, RequestFinishedMessageTemplate);

        private readonly RequestDelegate next;

        private readonly ILogger logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            HttpRequest request = httpContext.Request;
            HttpResponse response = httpContext.Response;

            LogRequestStarted(this.logger, request.Method, request.Path, null);

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                await this.next.Invoke(httpContext);
                stopwatch.Stop();

                if (response.StatusCode >= 200 && response.StatusCode <= 299)
                {
                    LogRequestFinishedInformation(this.logger, request.Method, request.Path, stopwatch.ElapsedMilliseconds, response.StatusCode, null);
                    return;
                }

                if (response.StatusCode >= 500)
                {
                    LogRequestFinishedError(this.logger, request.Method, request.Path, stopwatch.ElapsedMilliseconds, response.StatusCode, null);
                    return;
                }

                LogRequestFinishedWarning(this.logger, request.Method, request.Path, stopwatch.ElapsedMilliseconds, response.StatusCode, null);
            }
            catch (Exception exception)
            {
                stopwatch.Stop();

                LogRequestFinishedCritical(this.logger, request.Method, request.Path, stopwatch.ElapsedMilliseconds, "Unhandled Exception", exception);

                throw;
            }
        }
    }
}
