namespace Scaffold.WebApi.Middleware;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public partial class RequestLoggingMiddleware
{
    private readonly RequestDelegate next;

    private readonly ILogger logger;

    private readonly Options options;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IOptions<Options> options)
    {
        this.next = next;
        this.logger = logger;
        this.options = options.Value;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        HttpRequest request = httpContext.Request;
        HttpResponse response = httpContext.Response;

        if (IgnorePath(request.Path, this.options))
        {
            await this.next.Invoke(httpContext);
            return;
        }

        LogRequestStarted(this.logger, LogLevel.Information, request.Method, request.Path);

        try
        {
            await this.next.Invoke(httpContext);

            if (response.StatusCode is >= 200 and <= 299)
            {
                LogRequestFinished(this.logger, LogLevel.Information, request.Method, request.Path, response.StatusCode);
                return;
            }

            if (response.StatusCode >= 500)
            {
                LogRequestFinished(this.logger, LogLevel.Error, request.Method, request.Path, response.StatusCode);
                return;
            }

            LogRequestFinished(this.logger, LogLevel.Warning, request.Method, request.Path, response.StatusCode);
        }
        catch (Exception exception)
        {
            LogRequestException(this.logger, LogLevel.Critical, request.Method, request.Path, exception);
            throw;
        }
    }

    private static bool IgnorePath(PathString path, Options options)
    {
        return options.IgnorePatterns.Any(ignorePattern => Regex.IsMatch(path, ignorePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase));
    }

    [LoggerMessage(EventId = 1, Message = "Inbound HTTP {HttpMethod} {RequestPath} started")]
    private static partial void LogRequestStarted(ILogger logger, LogLevel logLevel, string httpMethod, PathString requestPath);

    [LoggerMessage(EventId = 2, Message = "Inbound HTTP {HttpMethod} {RequestPath} finished - {StatusCode}")]
    private static partial void LogRequestFinished(ILogger logger, LogLevel logLevel, string httpMethod, PathString requestPath, int StatusCode);

    [LoggerMessage(EventId = 3, Message = "Inbound HTTP {HttpMethod} {RequestPath} finished - Unhandled Exception")]
    private static partial void LogRequestException(ILogger logger, LogLevel logLevel, string httpMethod, PathString requestPath, Exception exception);

    public class Options
    {
        public string[] IgnorePatterns { get; set; } = Array.Empty<string>();
    }
}
