namespace Scaffold.WebApi.UnitTests.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
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
                mock.Object,
                Options.Create(new RequestLoggingMiddleware.Options()));

            HttpContext context = new DefaultHttpContext();
            context.Response.StatusCode = statusCode;

            // Act
            await middleware.Invoke(context);

            // Assert
            mock.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Equals("Inbound HTTP   started")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            mock.Verify(
                m => m.Log(
                    expectedLogLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Equals($"Inbound HTTP   finished - {statusCode}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once());
        }

        [Theory]
        [InlineData("/health", "^/health$", false)]
        [InlineData("/HEALTH", "^/health$", false)]
        [InlineData("/HeAlTh", "^/health$", false)]
        [InlineData("/health", "^/metrics$", true)]
        public async Task When_InvokingMiddlewareWithPathAndIgnorePattern_Expect_Logged(string path, string ignorePattern, bool logged)
        {
            // Arrange
            Mock<ILogger<RequestLoggingMiddleware>> mock = new Mock<ILogger<RequestLoggingMiddleware>>();
            mock.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            RequestLoggingMiddleware.Options options = new RequestLoggingMiddleware.Options
            {
                IgnorePatterns = new string[] { ignorePattern },
            };

            RequestLoggingMiddleware middleware = new RequestLoggingMiddleware(
                (httpContext) => Task.CompletedTask,
                mock.Object,
                Options.Create(options));

            HttpContext context = new DefaultHttpContext();
            context.Request.Path = path;

            // Act
            await middleware.Invoke(context);

            // Assert
            mock.Verify(
                m => m.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(logged ? 2 : 0));
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
                mock.Object,
                Options.Create(new RequestLoggingMiddleware.Options()));

            HttpContext context = new DefaultHttpContext();

            // Act
            Exception result = await Record.ExceptionAsync(() => middleware.Invoke(context));

            // Assert
            mock.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Equals("Inbound HTTP   started")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            mock.Verify(
                m => m.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Equals("Inbound HTTP   finished - Unhandled Exception")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            Assert.NotNull(result);
            Assert.Equal(exception, result);
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
}
