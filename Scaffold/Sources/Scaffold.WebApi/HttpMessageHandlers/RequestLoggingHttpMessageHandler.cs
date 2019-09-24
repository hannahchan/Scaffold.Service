namespace Scaffold.WebApi.HttpMessageHandlers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Scaffold.WebApi.Extensions;
    using Serilog.Context;
    using Serilog.Core;
    using Serilog.Events;

    public class RequestLoggingHttpMessageHandler : DelegatingHandler
    {
        private const string MessageTemplate = "Outbound HTTP {HttpMethod} {Uri} responded with {StatusCode} in {ElapsedMilliseconds}ms";

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
            LogLevel logLevel = LogLevel.Information;

            if (statusCode < 200 || statusCode > 299)
            {
                logLevel = LogLevel.Warning;
            }

            if (statusCode >= 500)
            {
                logLevel = LogLevel.Error;
            }

            using (LogContext.Push(new HttpResponseMessageEnricher(response)))
            {
                this.logger.Log(
                    logLevel,
                    MessageTemplate,
                    request.Method,
                    request.RequestUri,
                    statusCode,
                    stopwatch.ElapsedMilliseconds);
            }

            return response;
        }

        private class HttpResponseMessageEnricher : ILogEventEnricher
        {
            private readonly HttpResponseMessage response;

            public HttpResponseMessageEnricher(HttpResponseMessage response)
            {
                this.response = response;
            }

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                if (!this.response.IsSuccessStatusCode)
                {
                    Dictionary<string, string> headers = this.response.Headers.ToDictionary();

                    if (this.response.Content != null)
                    {
                        foreach (KeyValuePair<string, string> header in this.response.Content.Headers.ToDictionary())
                        {
                            headers.Add(header.Key, header.Value);
                        }
                    }

                    string body = this.response.Content?.ReadAsStringAsync().Result;

                    object httpResponse = new
                    {
                        Headers = headers,
                        Body = string.IsNullOrEmpty(body) ? null : body,
                        StatusCode = (int)this.response.StatusCode,
                        Version = this.response.Version.ToString(2),
                    };

                    logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("HttpResponse", httpResponse, true));
                }
            }
        }
    }
}
