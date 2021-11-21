namespace Scaffold.WebApi.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Scaffold.WebApi.HttpMessageHandlers;

internal static class HttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddRequestLogging(this IHttpClientBuilder builder)
    {
        builder.Services.AddTransient<RequestLoggingHttpMessageHandler>();
        return builder.AddHttpMessageHandler<RequestLoggingHttpMessageHandler>();
    }
}
