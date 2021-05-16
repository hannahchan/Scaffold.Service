namespace Scaffold.WebApi.Extensions
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Scaffold.WebApi.HttpMessageHandlers;

    internal static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddHttpClientMetrics(this IHttpClientBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddTransient<HttpClientMetricsMessageHandler>();
            return builder.AddHttpMessageHandler<HttpClientMetricsMessageHandler>();
        }

        public static IHttpClientBuilder AddRequestLogging(this IHttpClientBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddTransient<RequestLoggingHttpMessageHandler>();
            return builder.AddHttpMessageHandler<RequestLoggingHttpMessageHandler>();
        }
    }
}
