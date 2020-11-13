namespace Scaffold.WebApi.UnitTests.HttpMessageHandlers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
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
            Mock<ILogger<RequestLoggingHttpMessageHandler>> mock = new Mock<ILogger<RequestLoggingHttpMessageHandler>>();
            mock.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            RequestLoggingHttpMessageHandler handler = new RequestLoggingHttpMessageHandler(mock.Object)
            {
                InnerHandler = new MockResponseReturningInnerHandler(statusCode),
            };

            // Act
            using (HttpClient client = new HttpClient(handler))
            {
                await client.GetAsync(new Uri("http://localhost"));
            }

            // Assert
            mock.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Equals("Outbound HTTP GET http://localhost/ started")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once());

            mock.Verify(
                m => m.Log(
                    expectedLogLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Equals($"Outbound HTTP GET http://localhost/ finished - {statusCode}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once());
        }

        [Fact]
        public async Task When_SendingAsyncRespondsWithStatusCode_Expect_LogLevelCritical()
        {
            // Arrange
            Mock<ILogger<RequestLoggingHttpMessageHandler>> mock = new Mock<ILogger<RequestLoggingHttpMessageHandler>>();
            mock.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            Exception exception = new Exception();

            RequestLoggingHttpMessageHandler handler = new RequestLoggingHttpMessageHandler(mock.Object)
            {
                InnerHandler = new MockExceptionThrowingInnerHandler(exception),
            };

            Exception result;

            // Act
            using (HttpClient client = new HttpClient(handler))
            {
                result = await Record.ExceptionAsync(() => client.GetAsync(new Uri("http://localhost")));
            }

            // Assert
            mock.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Equals("Outbound HTTP GET http://localhost/ started")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            mock.Verify(
                m => m.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Equals("Outbound HTTP GET http://localhost/ finished - Unhandled Exception")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            Assert.NotNull(result);
            Assert.Equal(exception, result);
        }

        [Fact]
        public async Task When_SendingAsyncWithNullRequestUri_Expect_InvalidOperationException()
        {
            // Arrange
            TestRequestLoggingHttpMessageHandler handler = new TestRequestLoggingHttpMessageHandler();
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

        private class MockResponseReturningInnerHandler : DelegatingHandler
        {
            private readonly int statusCode;

            public MockResponseReturningInnerHandler(int statusCode)
            {
                this.statusCode = statusCode;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage { StatusCode = (HttpStatusCode)this.statusCode });
            }
        }

        private class MockExceptionThrowingInnerHandler : DelegatingHandler
        {
            private readonly Exception exception;

            public MockExceptionThrowingInnerHandler(Exception exception)
            {
                this.exception = exception;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromException<HttpResponseMessage>(this.exception);
            }
        }

        private class TestRequestLoggingHttpMessageHandler : RequestLoggingHttpMessageHandler
        {
            public TestRequestLoggingHttpMessageHandler()
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
