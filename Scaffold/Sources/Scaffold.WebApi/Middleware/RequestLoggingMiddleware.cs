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

                LogLevel logLevel = LogLevel.Information;

                if (response.StatusCode < 200 || response.StatusCode > 299)
                {
                    logLevel = LogLevel.Warning;
                }

                if (response.StatusCode >= 500)
                {
                    logLevel = LogLevel.Error;
                }

                if (request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase))
                {
                    logLevel = LogLevel.Debug;
                }

                this.logger.Log(
                    logLevel,
                    MessageTemplate,
                    request.Method,
                    request.Path,
                    response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                stopwatch.Stop();

                response.ContentType = "text/plain";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                this.logger.LogCritical(
                    exception,
                    MessageTemplate,
                    request.Method,
                    request.Path,
                    response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

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
