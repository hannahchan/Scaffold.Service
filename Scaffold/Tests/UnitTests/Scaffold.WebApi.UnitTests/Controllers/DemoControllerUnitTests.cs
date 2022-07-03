#pragma warning disable IDISP014 // Use a single instance of HttpClient

namespace Scaffold.WebApi.UnitTests.Controllers;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Scaffold.WebApi.Controllers;
using Xunit;

public class DemoControllerUnitTests
{
    public class ExampleRequest
    {
        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task When_InvokingExampleRequest_Expect_StatusCode(HttpStatusCode statusCode)
        {
            // Arrange
            using HttpClient httpClient = new HttpClient();

            DemoController.Options options = new DemoController.Options();
            DemoController.Client demoClient = new DemoController.Client(httpClient, Options.Create(options));
            DemoController controller = new DemoController(demoClient, Options.Create(options));

            // Act
            ActionResult result = await controller.ExampleRequest(statusCode: statusCode);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)statusCode, objectResult.StatusCode);

            ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("Example Request", problemDetails.Title);
            Assert.Equal((int)statusCode, problemDetails.Status);
        }
    }

    public class Trace
    {
        [Fact]
        public async Task When_InvokingTrace_Expect_Ok()
        {
            // Arrange
            using HttpRequestHandler httpRequestHandler = new HttpRequestHandler(new HttpResponseMessage());
            using HttpClient httpClient = new HttpClient(httpRequestHandler);

            DemoController.Options options = new DemoController.Options { NextHopBaseAddress = "http://localhost" };
            DemoController.Client demoClient = new DemoController.Client(httpClient, Options.Create(options));
            DemoController controller = new DemoController(demoClient, Options.Create(options));

            // Act
            ActionResult result = await controller.Trace();

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);

            ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("Example Trace", problemDetails.Title);
            Assert.Equal((int)HttpStatusCode.OK, problemDetails.Status);

            Assert.Collection(
                httpRequestHandler.ReceivedRequests,
                request =>
                {
                    Dictionary<string, StringValues> queryString = QueryHelpers.ParseQuery(request.RequestUri.Query);
                    Assert.Equal("1", queryString["depth"]);
                    Assert.Equal("1", queryString["fanOut"]);
                    Assert.Equal("True", queryString["sync"]);
                });
        }

        [Theory]
        [InlineData(-1, false)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(int.MinValue, false)]
        [InlineData(int.MaxValue, true)]
        public async Task When_InvokingTraceWithDepth_Expect_Ok(int depth, bool expectHttpRequestMessage)
        {
            // Arrange
            using HttpRequestHandler httpRequestHandler = new HttpRequestHandler(new HttpResponseMessage());
            using HttpClient httpClient = new HttpClient(httpRequestHandler);

            DemoController.Options options = new DemoController.Options() { NextHopBaseAddress = "http://localhost" };
            DemoController.Client demoClient = new DemoController.Client(httpClient, Options.Create(options));
            DemoController controller = new DemoController(demoClient, Options.Create(options));

            // Act
            ActionResult result = await controller.Trace(depth: depth);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);

            ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("Example Trace", problemDetails.Title);
            Assert.Equal((int)HttpStatusCode.OK, problemDetails.Status);

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
        [InlineData(9, 8)]
        [InlineData(10, 9)]
        [InlineData(11, 10)]
        public async Task When_InvokingTraceWithDepthAroundMaximum_Expect_Ok(int depth, int expectedDepth)
        {
            // Arrange
            using HttpRequestHandler httpRequestHandler = new HttpRequestHandler(new HttpResponseMessage());
            using HttpClient httpClient = new HttpClient(httpRequestHandler);

            DemoController.Options options = new DemoController.Options() { NextHopBaseAddress = "http://localhost", TraceMaxDepth = 10 };
            DemoController.Client demoClient = new DemoController.Client(httpClient, Options.Create(options));
            DemoController controller = new DemoController(demoClient, Options.Create(options));

            // Act
            ActionResult result = await controller.Trace(depth: depth);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);

            ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("Example Trace", problemDetails.Title);
            Assert.Equal((int)HttpStatusCode.OK, problemDetails.Status);

            Assert.Collection(
                httpRequestHandler.ReceivedRequests,
                request =>
                {
                    Dictionary<string, StringValues> queryString = QueryHelpers.ParseQuery(request.RequestUri.Query);
                    Assert.Equal(expectedDepth.ToString(), queryString["depth"]);
                });
        }

        [Fact]
        public async Task When_InvokingTraceWithNegativeMaximumDepth_Expect_Ok()
        {
            // Arrange
            using HttpRequestHandler httpRequestHandler = new HttpRequestHandler(new HttpResponseMessage());
            using HttpClient httpClient = new HttpClient(httpRequestHandler);

            DemoController.Options options = new DemoController.Options() { NextHopBaseAddress = "http://localhost", TraceMaxDepth = -10 };
            DemoController.Client demoClient = new DemoController.Client(httpClient, Options.Create(options));
            DemoController controller = new DemoController(demoClient, Options.Create(options));

            // Act
            ActionResult result = await controller.Trace();

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);

            ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("Example Trace", problemDetails.Title);
            Assert.Equal((int)HttpStatusCode.OK, problemDetails.Status);

            Assert.Empty(httpRequestHandler.ReceivedRequests);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task When_InvokingTraceWithFanOut_Expect_Ok(int fanOut)
        {
            // Arrange
            using HttpRequestHandler httpRequestHandler = new HttpRequestHandler(new HttpResponseMessage());
            using HttpClient httpClient = new HttpClient(httpRequestHandler);

            DemoController.Options options = new DemoController.Options { NextHopBaseAddress = "http://localhost" };
            DemoController.Client demoClient = new DemoController.Client(httpClient, Options.Create(options));
            DemoController controller = new DemoController(demoClient, Options.Create(options));

            // Act
            ActionResult result = await controller.Trace(fanOut: fanOut);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);

            ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("Example Trace", problemDetails.Title);
            Assert.Equal((int)HttpStatusCode.OK, problemDetails.Status);

            if (fanOut > 1)
            {
                Assert.Equal(fanOut, httpRequestHandler.ReceivedRequests.Count);
            }
            else
            {
                Assert.Collection(
                    httpRequestHandler.ReceivedRequests,
                    request =>
                    {
                        Dictionary<string, StringValues> queryString = QueryHelpers.ParseQuery(request.RequestUri.Query);
                        Assert.Equal("1", queryString["fanOut"]);
                    });
            }
        }

        [Theory]
        [InlineData(9, 9)]
        [InlineData(10, 10)]
        [InlineData(11, 10)]
        public async Task When_InvokingTraceWithFanOutAroundMaximum_Expect_Ok(int fanOut, int expectedFanOut)
        {
            // Arrange
            using HttpRequestHandler httpRequestHandler = new HttpRequestHandler(new HttpResponseMessage());
            using HttpClient httpClient = new HttpClient(httpRequestHandler);

            DemoController.Options options = new DemoController.Options() { NextHopBaseAddress = "http://localhost", TraceMaxFanOut = 10 };
            DemoController.Client demoClient = new DemoController.Client(httpClient, Options.Create(options));
            DemoController controller = new DemoController(demoClient, Options.Create(options));

            // Act
            ActionResult result = await controller.Trace(fanOut: fanOut);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);

            ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("Example Trace", problemDetails.Title);
            Assert.Equal((int)HttpStatusCode.OK, problemDetails.Status);

            Assert.Equal(expectedFanOut, httpRequestHandler.ReceivedRequests.Count);
        }

        [Fact]
        public async Task When_InvokingTraceWithNegativeMaximumFanOut_Expect_Ok()
        {
            // Arrange
            using HttpRequestHandler httpRequestHandler = new HttpRequestHandler(new HttpResponseMessage());
            using HttpClient httpClient = new HttpClient(httpRequestHandler);

            DemoController.Options options = new DemoController.Options() { NextHopBaseAddress = "http://localhost", TraceMaxFanOut = -10 };
            DemoController.Client demoClient = new DemoController.Client(httpClient, Options.Create(options));
            DemoController controller = new DemoController(demoClient, Options.Create(options));

            // Act
            ActionResult result = await controller.Trace();

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);

            ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("Example Trace", problemDetails.Title);
            Assert.Equal((int)HttpStatusCode.OK, problemDetails.Status);

            Assert.NotEmpty(httpRequestHandler.ReceivedRequests);
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
