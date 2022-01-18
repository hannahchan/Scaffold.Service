namespace Scaffold.WebApi.UnitTests.Middleware;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scaffold.WebApi.Middleware;
using Xunit;

public class RequestLoggingMiddlewareUnitTests
{
    [Theory]
    [InlineData(199, LogLevel.Warning)]
    [InlineData(200, LogLevel.Information)]
    [InlineData(299, LogLevel.Information)]
    [InlineData(300, LogLevel.Warning)]
    [InlineData(499, LogLevel.Warning)]
    [InlineData(500, LogLevel.Error)]
    [InlineData(501, LogLevel.Error)]
    public async Task When_InvokingMiddlewareWithStatusCode_Expect_LogLevel(int statusCode, LogLevel expectedLogLevel)
    {
        // Arrange
        bool requestDelegateInvoked = false;
        Mock.Logger<RequestLoggingMiddleware> logger = new Mock.Logger<RequestLoggingMiddleware>();

        RequestLoggingMiddleware middleware = new RequestLoggingMiddleware(
            (httpContext) =>
            {
                requestDelegateInvoked = true;
                return Task.CompletedTask;
            },
            logger,
            Options.Create(new RequestLoggingMiddleware.Options()));

        HttpContext context = new DefaultHttpContext();
        context.Response.StatusCode = statusCode;

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Collection(
            logger.LogEntries,
            logEntry =>
            {
                Assert.Equal(LogLevel.Information, logEntry.LogLevel);
                Assert.Equal("Inbound HTTP   started", logEntry.Message);
            },
            logEntry =>
            {
                Assert.Equal(expectedLogLevel, logEntry.LogLevel);
                Assert.Equal($"Inbound HTTP   finished - {statusCode}", logEntry.Message);
            });

        Assert.True(requestDelegateInvoked);
    }

    [Theory]
    [InlineData("/health", "^/health$", false)]
    [InlineData("/HEALTH", "^/health$", false)]
    [InlineData("/HeAlTh", "^/health$", false)]
    [InlineData("/health", "^/metrics$", true)]
    public async Task When_InvokingMiddlewareWithPathAndIgnorePattern_Expect_Logged(string path, string ignorePattern, bool logged)
    {
        // Arrange
        bool requestDelegateInvoked = false;
        Mock.Logger<RequestLoggingMiddleware> logger = new Mock.Logger<RequestLoggingMiddleware>();

        RequestLoggingMiddleware.Options options = new RequestLoggingMiddleware.Options
        {
            IgnorePatterns = new string[] { ignorePattern },
        };

        RequestLoggingMiddleware middleware = new RequestLoggingMiddleware(
            (httpContext) =>
            {
                requestDelegateInvoked = true;
                return Task.CompletedTask;
            },
            logger,
            Options.Create(options));

        HttpContext context = new DefaultHttpContext();
        context.Request.Path = path;

        // Act
        await middleware.Invoke(context);

        // Assert
        if (logged)
        {
            Assert.Equal(2, logger.LogEntries.Count);
        }
        else
        {
            Assert.Empty(logger.LogEntries);
        }

        Assert.True(requestDelegateInvoked);
    }

    [Fact]
    public async Task When_InvokingMiddlewareWithException_Expect_LogLevelCritical()
    {
        // Arrange
        Exception exception = new Exception("Unit Test");
        bool requestDelegateInvoked = false;
        Mock.Logger<RequestLoggingMiddleware> logger = new Mock.Logger<RequestLoggingMiddleware>();

        RequestLoggingMiddleware middleware = new RequestLoggingMiddleware(
            (httpContext) =>
            {
                requestDelegateInvoked = true;
                throw exception;
            },
            logger,
            Options.Create(new RequestLoggingMiddleware.Options()));

        HttpContext context = new DefaultHttpContext();

        // Act
        Exception result = await Record.ExceptionAsync(() => middleware.Invoke(context));

        // Assert
        Assert.Collection(
            logger.LogEntries,
            logEntry =>
            {
                Assert.Equal(LogLevel.Information, logEntry.LogLevel);
                Assert.Equal("Inbound HTTP   started", logEntry.Message);
            },
            logEntry =>
            {
                Assert.Equal(LogLevel.Critical, logEntry.LogLevel);
                Assert.Equal("Inbound HTTP   finished - Unhandled Exception", logEntry.Message);
            });

        Assert.NotNull(result);
        Assert.Equal(exception, result);
        Assert.True(requestDelegateInvoked);
    }

    [Fact]
    public void When_InstantiatingOptions_Expect_IgnorePatternsEmpty()
    {
        // Arrange
        RequestLoggingMiddleware.Options options;

        // Act
        options = new RequestLoggingMiddleware.Options();

        // Assert
        Assert.Empty(options.IgnorePatterns);
    }
}
