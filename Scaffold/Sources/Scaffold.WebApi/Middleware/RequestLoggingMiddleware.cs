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
        private const string MessageTemplate = "Inbound HTTP {HttpMethod} {Path} responded with {StatusCode} in {ElapsedMilliseconds}ms";

        private static readonly Action<ILogger, string, PathString, int, long, Exception?> LogCritical =
            LoggerMessage.Define<string, PathString, int, long>(LogLevel.Critical, default, MessageTemplate);

        private static readonly Action<ILogger, string, PathString, int, long, Exception?> LogDebug =
            LoggerMessage.Define<string, PathString, int, long>(LogLevel.Debug, default, MessageTemplate);

        private static readonly Action<ILogger, string, PathString, int, long, Exception?> LogError =
            LoggerMessage.Define<string, PathString, int, long>(LogLevel.Error, default, MessageTemplate);

        private static readonly Action<ILogger, string, PathString, int, long, Exception?> LogInformation =
            LoggerMessage.Define<string, PathString, int, long>(LogLevel.Information, default, MessageTemplate);

        private static readonly Action<ILogger, string, PathString, int, long, Exception?> LogWarning =
            LoggerMessage.Define<string, PathString, int, long>(LogLevel.Warning, default, MessageTemplate);

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

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                await this.next.Invoke(httpContext);
                stopwatch.Stop();

                if (request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase))
                {
                    LogDebug(this.logger, request.Method, request.Path, response.StatusCode, stopwatch.ElapsedMilliseconds, null);
                    return;
                }

                if (response.StatusCode >= 200 && response.StatusCode <= 299)
                {
                    LogInformation(this.logger, request.Method, request.Path, response.StatusCode, stopwatch.ElapsedMilliseconds, null);
                    return;
                }

                if (response.StatusCode >= 500)
                {
                    LogError(this.logger, request.Method, request.Path, response.StatusCode, stopwatch.ElapsedMilliseconds, null);
                    return;
                }

                LogWarning(this.logger, request.Method, request.Path, response.StatusCode, stopwatch.ElapsedMilliseconds, null);
            }
            catch (Exception exception)
            {
                stopwatch.Stop();

                response.ContentType = "text/plain";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                LogCritical(this.logger, request.Method, request.Path, response.StatusCode, stopwatch.ElapsedMilliseconds, exception);

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
