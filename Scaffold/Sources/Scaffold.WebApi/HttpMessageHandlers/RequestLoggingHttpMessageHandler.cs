namespace Scaffold.WebApi.HttpMessageHandlers;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public partial class RequestLoggingHttpMessageHandler : DelegatingHandler
{
    private readonly ILogger logger;

    public RequestLoggingHttpMessageHandler(ILogger<RequestLoggingHttpMessageHandler> logger)
    {
        this.logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri is null)
        {
            throw new InvalidOperationException("Missing RequestUri while processing request.");
        }

        LogRequestStarted(this.logger, LogLevel.Information, request.Method, request.RequestUri);

        try
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            int statusCode = (int)response.StatusCode;

            if (statusCode is >= 200 and <= 299)
            {
                LogRequestFinished(this.logger, LogLevel.Information, request.Method, request.RequestUri, statusCode);
                return response;
            }

            if (statusCode >= 500)
            {
                LogRequestFinished(this.logger, LogLevel.Error, request.Method, request.RequestUri, statusCode);
                return response;
            }

            LogRequestFinished(this.logger, LogLevel.Warning, request.Method, request.RequestUri, statusCode);

            return response;
        }
        catch (Exception exception)
        {
            LogRequestException(this.logger, LogLevel.Critical, request.Method, request.RequestUri, exception);

            throw;
        }
    }

    [LoggerMessage(EventId = 1, Message = "Outbound HTTP {HttpMethod} {Uri} started")]
    private static partial void LogRequestStarted(ILogger logger, LogLevel logLevel, HttpMethod httpMethod, Uri uri);

    [LoggerMessage(EventId = 2, Message = "Outbound HTTP {HttpMethod} {Uri} finished - {StatusCode}")]
    private static partial void LogRequestFinished(ILogger logger, LogLevel logLevel, HttpMethod httpMethod, Uri uri, int StatusCode);

    [LoggerMessage(EventId = 3, Message = "Outbound HTTP {HttpMethod} {Uri} finished - Unhandled Exception")]
    private static partial void LogRequestException(ILogger logger, LogLevel logLevel, HttpMethod httpMethod, Uri uri, Exception exception);
}
