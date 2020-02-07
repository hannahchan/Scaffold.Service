namespace Scaffold.WebApi.UnitTests.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Moq;
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
            Mock<ILogger<RequestLoggingMiddleware>> mock = new Mock<ILogger<RequestLoggingMiddleware>>();
            mock.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            RequestLoggingMiddleware middleware = new RequestLoggingMiddleware(
                (httpContext) => Task.CompletedTask,
                mock.Object);

            HttpContext context = new DefaultHttpContext();
            context.Response.StatusCode = statusCode;

            // Act
            await middleware.Invoke(context);

            // Assert
            mock.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Exactly(expectedLogLevel == LogLevel.Information ? 2 : 1));

            mock.Verify(
                m => m.Log(
                    expectedLogLevel,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Exactly(expectedLogLevel == LogLevel.Information ? 2 : 1));
        }

        [Fact]
        public async Task When_InvokingMiddlewareWithException_Expect_LogLevelCritical()
        {
            // Arrange
            Exception exception = new Exception("Unit Test");

            Mock<ILogger<RequestLoggingMiddleware>> mock = new Mock<ILogger<RequestLoggingMiddleware>>();
            mock.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            RequestLoggingMiddleware middleware = new RequestLoggingMiddleware(
                (httpContext) => throw exception,
                mock.Object);

            HttpContext context = new DefaultHttpContext();

            // Act
            Exception result = await Record.ExceptionAsync(() => middleware.Invoke(context));

            // Assert
            mock.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);

            mock.Verify(
                m => m.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    exception,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);

            Assert.NotNull(result);
            Assert.Equal(exception, result);
        }
    }
}
