#pragma warning disable IDISP014 // Use a single instance of HttpClient

namespace Scaffold.WebApi.UnitTests.Controllers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Scaffold.WebApi.Controllers;
using Xunit;

public class DemoControllerUnitTests
{
    public class Trace
    {
        [Fact]
        public async Task When_InvokingTrace_Expect_HierarchicalFormatTraceId()
        {
            // Arrange
            Activity.DefaultIdFormat = ActivityIdFormat.Hierarchical;
            using Activity activity = new Activity(nameof(activity));

            using HttpRequestHandler httpRequestHandler = new HttpRequestHandler(new HttpResponseMessage());
            using HttpClient httpClient = new HttpClient(httpRequestHandler);

            DemoController.Options options = new DemoController.Options { NextHopBaseAddress = "http://localhost" };
            DemoController.Client demoClient = new DemoController.Client(httpClient, Options.Create(options));
            DemoController controller = new DemoController(demoClient, Options.Create(options));

            // Act
            activity.Start();

            string result = await controller.Trace();

            // Assert
            Assert.Equal(activity.RootId, result);

            Assert.Collection(
                httpRequestHandler.ReceivedRequests,
                request =>
                {
                    Assert.EndsWith("/demo/trace/1", request.RequestUri.ToString());
                });
        }

        [Fact]
        public async Task When_InvokingTrace_Expect_W3CFormatTraceId()
        {
            // Arrange
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            using Activity activity = new Activity(nameof(activity));

            using HttpRequestHandler httpRequestHandler = new HttpRequestHandler(new HttpResponseMessage());
            using HttpClient httpClient = new HttpClient(httpRequestHandler);

            DemoController.Options options = new DemoController.Options { NextHopBaseAddress = "http://localhost" };
            DemoController.Client demoClient = new DemoController.Client(httpClient, Options.Create(options));
            DemoController controller = new DemoController(demoClient, Options.Create(options));

            // Act
            activity.Start();

            string result = await controller.Trace();

            // Assert
            Assert.Equal(activity.TraceId.ToHexString(), result);

            Assert.Collection(
                httpRequestHandler.ReceivedRequests,
                request =>
                {
                    Assert.EndsWith("/demo/trace/1", request.RequestUri.ToString());
                });
        }

        [Fact]
        public async Task When_InvokingTrace_Expect_NullTraceId()
        {
            // Arrange
            using HttpRequestHandler httpRequestHandler = new HttpRequestHandler(new HttpResponseMessage());
            using HttpClient httpClient = new HttpClient(httpRequestHandler);

            DemoController.Options options = new DemoController.Options { NextHopBaseAddress = "http://localhost" };
            DemoController.Client demoClient = new DemoController.Client(httpClient, Options.Create(options));
            DemoController controller = new DemoController(demoClient, Options.Create(options));

            // Act
            string result = await controller.Trace();

            // Assert
            Assert.Null(result);

            Assert.Collection(
                httpRequestHandler.ReceivedRequests,
                request =>
                {
                    Assert.EndsWith("/demo/trace/1", request.RequestUri.ToString());
                });
        }

        [Theory]
        [InlineData(-1, false)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(int.MinValue, false)]
        [InlineData(int.MaxValue, true)]
        public async Task When_InvokingTrace_Expect_HttpRequestMessage(int depth, bool expectHttpRequestMessage)
        {
            // Arrange
            using HttpRequestHandler httpRequestHandler = new HttpRequestHandler(new HttpResponseMessage());
            using HttpClient httpClient = new HttpClient(httpRequestHandler);

            DemoController.Options options = new DemoController.Options() { NextHopBaseAddress = "http://localhost" };
            DemoController.Client demoClient = new DemoController.Client(httpClient, Options.Create(options));
            DemoController controller = new DemoController(demoClient, Options.Create(options));

            // Act
            await controller.Trace(depth);

            // Assert
            if (expectHttpRequestMessage)
            {
                Assert.Collection(
                    httpRequestHandler.ReceivedRequests,
                    request => Assert.StartsWith("http://localhost/demo/trace", request.RequestUri.ToString()));
            }
            else
            {
                Assert.Empty(httpRequestHandler.ReceivedRequests);
            }
        }

        [Theory]
        [InlineData(9, "/demo/trace/8")]
        [InlineData(10, "/demo/trace/9")]
        [InlineData(11, "/demo/trace/10")]
        [InlineData(12, "/demo/trace/10")]
        public async Task When_InvokingTrace_Expect_Path(int depth, string expectedPath)
        {
            // Arrange
            using HttpRequestHandler httpRequestHandler = new HttpRequestHandler(new HttpResponseMessage());
            using HttpClient httpClient = new HttpClient(httpRequestHandler);

            DemoController.Options options = new DemoController.Options() { NextHopBaseAddress = "http://localhost", MaxDepth = 10 };
            DemoController.Client demoClient = new DemoController.Client(httpClient, Options.Create(options));
            DemoController controller = new DemoController(demoClient, Options.Create(options));

            // Act
            await controller.Trace(depth);

            // Assert
            Assert.Collection(
                httpRequestHandler.ReceivedRequests,
                request =>
                {
                    Assert.EndsWith(expectedPath, request.RequestUri.ToString());
                });
        }
    }

    private class HttpRequestHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage response;

        public HttpRequestHandler(HttpResponseMessage response)
        {
            this.response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public List<HttpRequestMessage> ReceivedRequests { get; } = new List<HttpRequestMessage>();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.ReceivedRequests.Add(request);
            this.response.RequestMessage = request;

            return Task.FromResult(this.response);
        }
    }
}
