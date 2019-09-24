namespace Scaffold.WebApi.UnitTests.HttpMessageHandlers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
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
        public async Task When_SendingAsync_Expect_LogLevel(int statusCode, LogLevel expectedLogLevel)
        {
            // Arrange
            Mock<ILogger<RequestLoggingHttpMessageHandler>> mock = new Mock<ILogger<RequestLoggingHttpMessageHandler>>();

            // Act
            using (RequestLoggingHttpMessageHandler handler = new RequestLoggingHttpMessageHandler(mock.Object)
            {
                InnerHandler = new InnerHandler(statusCode),
            })
            using (HttpClient client = new HttpClient(handler))
            {
                (await client.GetAsync(new Uri("http://localhost"))).Dispose();
            }

            // Assert
            mock.Verify(
                m => m.Log(
                    expectedLogLevel,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        private class InnerHandler : DelegatingHandler
        {
            private readonly int statusCode;

            public InnerHandler(int statusCode)
            {
                this.statusCode = statusCode;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return await Task.FromResult(new HttpResponseMessage { StatusCode = (HttpStatusCode)this.statusCode });
            }
        }
    }
}
