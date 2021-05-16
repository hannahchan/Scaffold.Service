namespace Scaffold.WebApi.UnitTests.HttpMessageHandlers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Prometheus;
    using Scaffold.WebApi.HttpMessageHandlers;
    using Xunit;

    public class HttpClientMetricsMessageHandlerUnitTests
    {
        [Fact]
        public async Task When_SendingAsync_Expect_MetricsCollected()
        {
            // Arrange
            HttpClientMetricsMessageHandler handler = new HttpClientMetricsMessageHandler()
            {
                InnerHandler = new MockResponseReturningInnerHandler((int)HttpStatusCode.OK),
            };

            // Act
            using (HttpClient client = new HttpClient(handler))
            {
                await client.GetAsync(new Uri("http://localhost"));
            }

            // Assert
            MemoryStream stream = new MemoryStream();
            await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);
            stream.Position = 0;

            string result;

            using (StreamReader reader = new StreamReader(stream))
            {
                result = await reader.ReadToEndAsync();
            }

            Assert.Contains("# HELP httpclient_requests_in_progress Number of requests currently being executed by an HttpClient.", result);
            Assert.Contains("# TYPE httpclient_requests_in_progress gauge", result);
            Assert.Contains("httpclient_requests_in_progress{method=\"GET\",host=\"localhost\"} 0", result);

            Assert.Contains("# HELP httpclient_requests_received_total Count of HTTP requests that have been completed by an HttpClient.", result);
            Assert.Contains("# TYPE httpclient_requests_received_total counter", result);
            Assert.Contains("httpclient_requests_received_total{method=\"GET\",host=\"localhost\",code=\"200\"} 1", result);

            Assert.Contains("# HELP httpclient_request_duration_seconds Duration histogram of HTTP requests performed by an HttpClient.", result);
            Assert.Contains("# TYPE httpclient_request_duration_seconds histogram", result);
            Assert.Contains("httpclient_request_duration_seconds_sum{method=\"GET\",host=\"localhost\",code=\"200\"}", result);
            Assert.Contains("httpclient_request_duration_seconds_count{method=\"GET\",host=\"localhost\",code=\"200\"} 1", result);
            Assert.Contains("httpclient_request_duration_seconds_bucket{method=\"GET\",host=\"localhost\",code=\"200\",le=\"0.001\"} 1", result);
            Assert.Contains("httpclient_request_duration_seconds_bucket{method=\"GET\",host=\"localhost\",code=\"200\",le=\"32.768\"} 1", result);
            Assert.Contains("httpclient_request_duration_seconds_bucket{method=\"GET\",host=\"localhost\",code=\"200\",le=\"+Inf\"} 1", result);
        }

        [Fact]
        public async Task When_SendingAsyncWithNullRequestUri_Expect_InvalidOperationException()
        {
            // Arrange
            TestHttpClientMetricsMessageHandler handler = new TestHttpClientMetricsMessageHandler();
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

        private class TestHttpClientMetricsMessageHandler : HttpClientMetricsMessageHandler
        {
            public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
