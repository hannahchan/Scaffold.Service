namespace Scaffold.WebApi.UnitTests.Middleware
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.FileProviders;
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
                new TestWebHostEnvironment { ApplicationName = "Unit Test", EnvironmentName = "Production" },
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

        [Theory]
        [InlineData("/health")]
        [InlineData("/Health")]
        [InlineData("/HEALTH")]
        public async Task When_InvokingMiddlewareWithHealthCheckPath_Expect_LogLevelDebug(string path)
        {
            // Arrange
            Mock<ILogger<RequestLoggingMiddleware>> mock = new Mock<ILogger<RequestLoggingMiddleware>>();
            mock.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            RequestLoggingMiddleware middleware = new RequestLoggingMiddleware(
                (httpContext) => Task.CompletedTask,
                new TestWebHostEnvironment { ApplicationName = "Unit Test", EnvironmentName = "Production" },
                mock.Object);

            HttpContext context = new DefaultHttpContext();
            context.Request.Path = path;

            // Act
            await middleware.Invoke(context);

            // Assert
            mock.Verify(
                m => m.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task When_InvokingMiddlewareInDevelopmentWithException_Expect_LogLevelCritical()
        {
            // Arrange
            Exception exception = new Exception("Unit Test");

            Mock<ILogger<RequestLoggingMiddleware>> mock = new Mock<ILogger<RequestLoggingMiddleware>>();
            mock.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            RequestLoggingMiddleware middleware = new RequestLoggingMiddleware(
                (httpContext) => throw exception,
                new TestWebHostEnvironment { ApplicationName = "Unit Test", EnvironmentName = "Development" },
                mock.Object);

            HttpContext context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

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
                Times.Once);

            mock.Verify(
                m => m.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    exception,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);

            Assert.Equal("text/plain", context.Response.ContentType);
            Assert.Equal(500, context.Response.StatusCode);

            using StreamReader reader = new StreamReader(context.Response.Body);
            context.Response.Body.Position = 0;
            Assert.Equal(exception.ToString(), reader.ReadToEnd());
        }

        [Fact]
        public async Task When_InvokingMiddlewareInProductionWithException_Expect_LogLevelCritical()
        {
            // Arrange
            Exception exception = new Exception("Unit Test");

            Mock<ILogger<RequestLoggingMiddleware>> mock = new Mock<ILogger<RequestLoggingMiddleware>>();
            mock.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            RequestLoggingMiddleware middleware = new RequestLoggingMiddleware(
                (httpContext) => throw exception,
                new TestWebHostEnvironment { ApplicationName = "Unit Test", EnvironmentName = "Production" },
                mock.Object);

            HttpContext context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

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
                Times.Once);

            mock.Verify(
                m => m.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    exception,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);

            Assert.Equal("text/plain", context.Response.ContentType);
            Assert.Equal(500, context.Response.StatusCode);

            using StreamReader reader = new StreamReader(context.Response.Body);
            context.Response.Body.Position = 0;
            Assert.Equal("Oh no! Something has gone wrong.", reader.ReadToEnd());
        }

        private class TestWebHostEnvironment : IWebHostEnvironment
        {
            public string EnvironmentName { get; set; } = null!;

            public string ApplicationName { get; set; } = null!;

            public string WebRootPath { get; set; } = null!;

            public IFileProvider WebRootFileProvider { get; set; } = null!;

            public string ContentRootPath { get; set; } = null!;

            public IFileProvider ContentRootFileProvider { get; set; } = null!;
        }
    }
}
