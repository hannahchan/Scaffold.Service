namespace Scaffold.WebApi.UnitTests.HttpMessageHandlers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Scaffold.WebApi.HttpMessageHandlers;
    using Xunit;

    public class RequestLoggingHttpMessageHandlerUnitTests
    {
        [Theory]
        [InlineData(199, LogLevel.Warning)]
        [InlineData(200, LogLevel.Information)]
        [InlineData(299, LogLevel.Information)]
        [InlineData(300, LogLevel.Warning)]
        [InlineData(499, LogLevel.Warning)]
        [InlineData(500, LogLevel.Error)]
        [InlineData(501, LogLevel.Error)]
        public async Task When_SendingAsyncRespondsWithStatusCode_Expect_LogLevel(int statusCode, LogLevel expectedLogLevel)
        {
            // Arrange
            Mock.Logger<RequestLoggingHttpMessageHandler> logger = new Mock.Logger<RequestLoggingHttpMessageHandler>();

            RequestLoggingHttpMessageHandler handler = new RequestLoggingHttpMessageHandler(logger)
            {
                InnerHandler = new Mock.HttpRequestHandler(new HttpResponseMessage { StatusCode = (HttpStatusCode)statusCode }),
            };

            // Act
            using (HttpClient client = new HttpClient(handler))
            {
                await client.GetAsync(new Uri("http://localhost"));
            }

            // Assert
            Assert.Collection(
                logger.LogEntries,
                logEntry =>
                {
                    Assert.Equal(LogLevel.Information, logEntry.LogLevel);
                    Assert.Equal("Outbound HTTP GET http://localhost/ started", logEntry.Message);
                },
                logEntry =>
                {
                    Assert.Equal(expectedLogLevel, logEntry.LogLevel);
                    Assert.Equal($"Outbound HTTP GET http://localhost/ finished - {statusCode}", logEntry.Message);
                });
        }

        [Fact]
        public async Task When_SendingAsyncRespondsWithStatusCode_Expect_LogLevelCritical()
        {
            // Arrange
            Mock.Logger<RequestLoggingHttpMessageHandler> logger = new Mock.Logger<RequestLoggingHttpMessageHandler>();
            Exception exception = new Exception();

            RequestLoggingHttpMessageHandler handler = new RequestLoggingHttpMessageHandler(logger)
            {
                InnerHandler = new Mock.HttpRequestHandler(exception),
            };

            Exception result;

            // Act
            using (HttpClient client = new HttpClient(handler))
            {
                result = await Record.ExceptionAsync(() => client.GetAsync(new Uri("http://localhost")));
            }

            // Assert
            Assert.Collection(
                logger.LogEntries,
                logEntry =>
                {
                    Assert.Equal(LogLevel.Information, logEntry.LogLevel);
                    Assert.Equal("Outbound HTTP GET http://localhost/ started", logEntry.Message);
                },
                logEntry =>
                {
                    Assert.Equal(LogLevel.Critical, logEntry.LogLevel);
                    Assert.Equal("Outbound HTTP GET http://localhost/ finished - Unhandled Exception", logEntry.Message);
                });

            Assert.NotNull(result);
            Assert.Equal(exception, result);
        }

        [Fact]
        public async Task When_SendingAsyncWithNullRequestUri_Expect_InvalidOperationException()
        {
            // Arrange
            WrappedRequestLoggingHttpMessageHandler handler = new WrappedRequestLoggingHttpMessageHandler();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = null,
            };

            // Act
            Exception result = await Record.ExceptionAsync(() => handler.SendAsync(request, default));

            // Assert
            Assert.NotNull(result);
            InvalidOperationException invalidOperationException = Assert.IsType<InvalidOperationException>(result);
            Assert.Equal("Missing RequestUri while processing request.", invalidOperationException.Message);
        }

        // Required to call protected methods
        private class WrappedRequestLoggingHttpMessageHandler : RequestLoggingHttpMessageHandler
        {
            public WrappedRequestLoggingHttpMessageHandler()
                : base(new NullLogger<RequestLoggingHttpMessageHandler>())
            {
            }

            public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
