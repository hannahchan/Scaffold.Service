namespace Scaffold.WebApi.IntegrationTests.Controllers;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.WebApi.Controllers;
using Xunit;

public class DemoControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    public class Trace : DemoControllerIntegrationTests
    {
        private readonly WebApplicationFactory<Program> factory;

        public Trace(WebApplicationFactory<Program> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder => builder
                .ConfigureWithDefaultsForTesting()
                .ConfigureServices(services =>
                    services.AddTransient<DemoController.IClient, MockDemoClient>()));
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
            Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task When_InvokingTraceWithDepth_Expect_Ok()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();

            // Act
            using HttpResponseMessage response = await client.GetAsync($"/demo/trace/{new Random().Next()}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task When_InvokingTraceWithDepth_Expect_BadRequest()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();

            // Act
            using HttpResponseMessage response = await client.GetAsync($"/demo/trace/abc");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType.MediaType);
        }
    }

    private class MockDemoClient : DemoController.IClient
    {
        public Task Trace(int depth, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
