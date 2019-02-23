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

        private string messageTemplate =
            $"HTTP {{Method}} {{Path}} responded with {{StatusCode}} in {{ElapsedMilliseconds}} ms";

        public RequestLoggingMiddleware(RequestDelegate next, IHostingEnvironment env, ILogger<RequestLoggingMiddleware> logger)
        {
            this.next = next;
            this.env = env;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            using (LogContext.Push(new ApplicationNameEnricher(this.env)))
            using (LogContext.Push(new HttpContextEnricher(httpContext)))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    await this.next.Invoke(httpContext);
                    stopwatch.Stop();

                    using (LogContext.Push(new HttpResponseStatusCodeEnricher(httpContext.Response)))
                    using (LogContext.PushProperty(nameof(Stopwatch.ElapsedMilliseconds), stopwatch.ElapsedMilliseconds))
                    using (LogContext.Push(new HttpRequestHeadersEnricher(httpContext.Request)))
                    {
                        LogLevel logLevel = LogLevel.Information;

                        if (httpContext.Response.StatusCode < 200 && httpContext.Response.StatusCode > 299)
                        {
                            logLevel = LogLevel.Warning;
                        }

                        if (httpContext.Response.StatusCode >= 500)
                        {
                            logLevel = LogLevel.Error;
                        }

                        this.logger.Log(logLevel, this.messageTemplate);
                    }
                }
                catch (Exception exception)
                {
                    stopwatch.Stop();

                    httpContext.Response.ContentType = "text/plain";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    using (LogContext.Push(new HttpResponseStatusCodeEnricher(httpContext.Response)))
                    using (LogContext.PushProperty(nameof(Stopwatch.ElapsedMilliseconds), stopwatch.ElapsedMilliseconds))
                    using (LogContext.Push(new HttpRequestHeadersEnricher(httpContext.Request)))
                    {
                        this.logger.LogCritical(exception, this.messageTemplate);
                    }

                    if (this.env.IsDevelopment())
                    {
                        await httpContext.Response.WriteAsync(exception.ToString());
                        return;
                    }

                    await httpContext.Response.WriteAsync("Oh no! Something has gone wrong.");
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

            public HttpContextEnricher(HttpContext httpContext) => this.httpContext = httpContext;

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(
                    nameof(HttpContext.Connection.RemoteIpAddress), this.httpContext.Connection.RemoteIpAddress));

                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(
                    nameof(HttpContext.Connection.LocalIpAddress), this.httpContext.Connection.LocalIpAddress));

                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(
                    nameof(HttpContext.Request.Method), this.httpContext.Request.Method));

                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(
                    nameof(HttpContext.Request.Scheme), this.httpContext.Request.Scheme));

                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(
                    nameof(HttpContext.Request.Host), this.httpContext.Request.Host));

                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(
                    nameof(HttpContext.Request.Path), this.httpContext.Request.Path));
            }
        }

        private class HttpRequestHeadersEnricher : ILogEventEnricher
        {
            private readonly IDictionary headers;

            public HttpRequestHeadersEnricher(HttpRequest httpRequest) =>
                this.headers = httpRequest.Headers
                    .Where(header => !header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(header => header.Key, header => header.Value.ToString());

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) =>
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(nameof(HttpRequest.Headers), this.headers, true));
        }

        private class HttpResponseStatusCodeEnricher : ILogEventEnricher
        {
            private readonly HttpResponse httpResponse;

            public HttpResponseStatusCodeEnricher(HttpResponse httpResponse) => this.httpResponse = httpResponse;

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) =>
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(
                    nameof(HttpResponse.StatusCode), this.httpResponse.StatusCode));
        }
    }
}
