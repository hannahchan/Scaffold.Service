namespace Scaffold.WebApi.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class RequestLoggingMiddleware
    {
        private const string RequestStartedMessageTemplate = "Inbound HTTP {HttpMethod} {Path} started";

        private const string RequestFinishedMessageTemplate = "Inbound HTTP {HttpMethod} {Path} finished in {ElapsedMilliseconds}ms - {StatusCode}";

        private static readonly Action<ILogger, string, PathString, Exception?> LogRequestStartedDebug =
            LoggerMessage.Define<string, PathString>(LogLevel.Debug, default, RequestStartedMessageTemplate);

        private static readonly Action<ILogger, string, PathString, Exception?> LogRequestStartedInformation =
            LoggerMessage.Define<string, PathString>(LogLevel.Information, default, RequestStartedMessageTemplate);

        private static readonly Action<ILogger, string, PathString, long, int, Exception?> LogRequestFinishedCritical =
            LoggerMessage.Define<string, PathString, long, int>(LogLevel.Critical, default, RequestFinishedMessageTemplate);

        private static readonly Action<ILogger, string, PathString, long, int, Exception?> LogRequestFinishedDebug =
            LoggerMessage.Define<string, PathString, long, int>(LogLevel.Debug, default, RequestFinishedMessageTemplate);

        private static readonly Action<ILogger, string, PathString, long, int, Exception?> LogRequestFinishedError =
            LoggerMessage.Define<string, PathString, long, int>(LogLevel.Error, default, RequestFinishedMessageTemplate);

        private static readonly Action<ILogger, string, PathString, long, int, Exception?> LogRequestFinishedInformation =
            LoggerMessage.Define<string, PathString, long, int>(LogLevel.Information, default, RequestFinishedMessageTemplate);

        private static readonly Action<ILogger, string, PathString, long, int, Exception?> LogRequestFinishedWarning =
            LoggerMessage.Define<string, PathString, long, int>(LogLevel.Warning, default, RequestFinishedMessageTemplate);

        private readonly RequestDelegate next;

        private readonly IWebHostEnvironment env;

        private readonly ILogger logger;

        public RequestLoggingMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<RequestLoggingMiddleware> logger)
        {
            this.next = next;
            this.env = env;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            HttpRequest request = httpContext.Request;
            HttpResponse response = httpContext.Response;

            if (request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase))
            {
                LogRequestStartedDebug(this.logger, request.Method, request.Path, null);
            }
            else
            {
                LogRequestStartedInformation(this.logger, request.Method, request.Path, null);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                await this.next.Invoke(httpContext);
                stopwatch.Stop();

                if (request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase))
                {
                    LogRequestFinishedDebug(this.logger, request.Method, request.Path, stopwatch.ElapsedMilliseconds, response.StatusCode, null);
                    return;
                }

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

                response.ContentType = "text/plain";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                LogRequestFinishedCritical(this.logger, request.Method, request.Path, stopwatch.ElapsedMilliseconds, response.StatusCode, exception);

                if (this.env.IsDevelopment())
                {
                    await response.WriteAsync(exception.ToString());
                    return;
                }

                await response.WriteAsync("Oh no! Something has gone wrong.");
            }
        }
    }
}
