namespace Scaffold.WebApi.IntegrationTests.Controllers;

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.WebApi.Controllers;
using Xunit;

public class DemoControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public DemoControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory.WithWebHostBuilder(builder => builder
            .ConfigureWithDefaultsForTesting()
            .ConfigureServices(services =>
                services.AddTransient<DemoController.IClient, MockDemoClient>()));
    }

    public class ExampleRequest : DemoControllerIntegrationTests
    {
        public ExampleRequest(WebApplicationFactory<Program> factory)
            : base(factory)
        {
        }

        [Theory]
        [InlineData(nameof(HttpMethod.Delete))]
        [InlineData(nameof(HttpMethod.Get))]
        [InlineData(nameof(HttpMethod.Post))]
        [InlineData(nameof(HttpMethod.Put))]
        public async Task When_InvokingExampleRequestWithHttpMethod_Expect_Ok(string method)
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();
            using HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), "/demo/request");

            // Act
            using HttpResponseMessage response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType.MediaType);
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task When_InvokingExampleRequestWithHttpStatusCodeInteger_Expect_StatusCode(HttpStatusCode statusCode)
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"/demo/request?statusCode={(int)statusCode}");

            // Act
            using HttpResponseMessage response = await client.SendAsync(request);

            // Assert
            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType.MediaType);
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task When_InvokingExampleRequestWithHttpStatusCodeString_Expect_StatusCode(HttpStatusCode statusCode)
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"/demo/request?statusCode={statusCode}");

            // Act
            using HttpResponseMessage response = await client.SendAsync(request);

            // Assert
            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType.MediaType);
        }
    }

    public class Trace : DemoControllerIntegrationTests
    {
        public Trace(WebApplicationFactory<Program> factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task When_InvokingTrace_Expect_Ok()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();

            // Act
            using HttpResponseMessage response = await client.GetAsync("/demo/trace");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task When_InvokingTraceWithDepth_Expect_Ok()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();

            // Act
            using HttpResponseMessage response = await client.GetAsync($"/demo/trace?depth={new Random().Next()}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task When_InvokingTraceWithDepth_Expect_BadRequest()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();

            // Act
            using HttpResponseMessage response = await client.GetAsync($"/demo/trace?depth=abc");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task When_InvokingTraceWithFanOut_Expect_Ok()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();

            // Act
            using HttpResponseMessage response = await client.GetAsync($"/demo/trace?fanOut={new Random().Next()}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task When_InvokingTraceWithFanOut_Expect_BadRequest()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();

            // Act
            using HttpResponseMessage response = await client.GetAsync($"/demo/trace?fanOut=abc");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task When_InvokingTraceWithSync_Expect_Ok()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();

            // Act
            using HttpResponseMessage response = await client.GetAsync($"/demo/trace?sync=true");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task When_InvokingTraceWithSync_Expect_BadRequest()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();

            // Act
            using HttpResponseMessage response = await client.GetAsync($"/demo/trace?sync=abc");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType.MediaType);
        }
    }

    private class MockDemoClient : DemoController.IClient
    {
        public Task Trace(int depth, int fanOut, bool sync, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
