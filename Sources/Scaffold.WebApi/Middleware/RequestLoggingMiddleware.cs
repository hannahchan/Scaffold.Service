namespace Scaffold.WebApi.Middleware
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Serilog.Context;
    using Serilog.Core;
    using Serilog.Events;

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly IHostingEnvironment env;

        private readonly ILogger logger;

        public RequestLoggingMiddleware(RequestDelegate next, IHostingEnvironment env, ILogger<RequestLoggingMiddleware> logger)
        {
            this.next = next;
            this.env = env;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            using (LogContext.Push(new ApplicationNameEnricher(this.env)))
            {
                HttpRequest request = httpContext.Request;
                HttpResponse response = httpContext.Response;

                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    await this.next.Invoke(httpContext);
                    stopwatch.Stop();

                    using (LogContext.Push(new HttpContextEnricher(httpContext, stopwatch.ElapsedMilliseconds)))
                    {
                        LogLevel logLevel = LogLevel.Information;

                        if (response.StatusCode < 200 && response.StatusCode > 299)
                        {
                            logLevel = LogLevel.Warning;
                        }

                        if (response.StatusCode >= 500)
                        {
                            logLevel = LogLevel.Error;
                        }

                        this.logger.Log(logLevel, $"HTTP {request.Method} {request.Path} responded with {response.StatusCode} in {stopwatch.ElapsedMilliseconds} ms");
                    }
                }
                catch (Exception exception)
                {
                    stopwatch.Stop();

                    using (LogContext.Push(new HttpContextEnricher(httpContext, stopwatch.ElapsedMilliseconds)))
                    {
                        this.logger.LogCritical(exception, $"HTTP {request.Method} {request.Path} responded with {response.StatusCode} in {stopwatch.ElapsedMilliseconds} ms");
                    }

                    response.ContentType = "text/plain";
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    if (this.env.IsDevelopment())
                    {
                        await response.WriteAsync(exception.ToString());
                        return;
                    }

                    await response.WriteAsync("Oh no! Something has gone wrong.");
                }
            }
        }

        private class ApplicationNameEnricher : ILogEventEnricher
        {
            private readonly IHostingEnvironment env;

            public ApplicationNameEnricher(IHostingEnvironment env) => this.env = env;

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) =>
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(
                    nameof(IHostingEnvironment.ApplicationName), this.env.ApplicationName));
        }

        private class HttpContextEnricher : ILogEventEnricher
        {
            private readonly HttpContext httpContext;

            private readonly long? elapsedMilliseconds;

            public HttpContextEnricher(HttpContext httpContext, long? elapsedMilliseconds = null)
            {
                this.httpContext = httpContext;
                this.elapsedMilliseconds = elapsedMilliseconds;
            }

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                object httpConnection = new
                {
                    RemoteIpAddress = this.httpContext.Connection.RemoteIpAddress.ToString(),
                    LocalIpAddress = this.httpContext.Connection.LocalIpAddress.ToString()
                };

                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("HttpConnection", httpConnection, true));

                object httpRequest = new
                {
                    Method = this.httpContext.Request.Method,
                    Scheme = this.httpContext.Request.Scheme,
                    Host = this.httpContext.Request.Host.ToString(),
                    Path = this.httpContext.Request.Path.ToString(),
                    Headers = this.httpContext.Request.Headers
                        .Where(header => !header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                        .ToDictionary(header => header.Key, header => header.Value.ToString())
                };

                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("HttpRequest", httpRequest, true));

                object httpResponse = new
                {
                    StatusCode = this.httpContext.Response.StatusCode,
                    ElapsedMilliseconds = this.elapsedMilliseconds
                };

                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("HttpResponse", httpResponse, true));
            }
        }
    }
}
