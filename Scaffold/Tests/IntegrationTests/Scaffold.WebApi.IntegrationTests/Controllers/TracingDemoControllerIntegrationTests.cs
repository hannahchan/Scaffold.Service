namespace Scaffold.WebApi.IntegrationTests.Controllers;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Scaffold.WebApi.Controllers;
using Xunit;

public class TracingDemoControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
{
    public class Proxy : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public Proxy(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder =>
            {
                builder
                    .ConfigureWithDefaultsForTesting()
                    .ConfigureServices(services =>
                        services.AddTransient<TracingDemoController.IClient, MockTracingDemoClient>());
            });
        }

        [Fact]
        public async Task When_SayingHelloToName_Expect_HelloMessage()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();
            string name = Guid.NewGuid().ToString();

            // Act
            using HttpResponseMessage response = await client.GetAsync($"/TracingDemo?name={name}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.MediaType);
            Assert.Equal($"Hello, {name}!", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task When_SayingHelloToNullName_Expect_HelloMessage()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();

            // Act
            using HttpResponseMessage response = await client.GetAsync($"/TracingDemo");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.MediaType);
            Assert.Equal($"Hello, random!", await response.Content.ReadAsStringAsync());
        }

        private class MockTracingDemoClient : TracingDemoController.IClient
        {
            public Task<HttpResponseMessage> Get(Uri uri)
            {
                Dictionary<string, StringValues> queryParameters = QueryHelpers.ParseQuery(uri.Query);

                return Task.FromResult(new HttpResponseMessage
                {
                    Content = new StringContent($"Hello, {queryParameters["name"]}!"),
                });
            }
        }
    }

    public class Hello : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public Hello(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder => builder.ConfigureWithDefaultsForTesting());
        }

        [Fact]
        public async Task When_SayingHelloToName_Expect_HelloMessage()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();
            string name = Guid.NewGuid().ToString();

            HttpResponseMessage response = null;

            // Act
            do
            {
                response?.Dispose();
                response = await client.GetAsync($"/TracingDemo/Hello?name={name}");
            }
            while (!response.IsSuccessStatusCode);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.MediaType);
            Assert.Equal($"Hello, {name}!", await response.Content.ReadAsStringAsync());

            response.Dispose();
        }

        [Fact]
        public async Task When_SayingHelloToNullName_Expect_HelloMessage()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();

            HttpResponseMessage response = null;

            // Act
            do
            {
                response?.Dispose();
                response = await client.GetAsync($"/TracingDemo/Hello");
            }
            while (!response.IsSuccessStatusCode);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.MediaType);
            Assert.Equal($"Hello, random!", await response.Content.ReadAsStringAsync());

            response.Dispose();
        }

        [Fact]
        public async Task When_SayingHello_Expect_ServiceUnavailable()
        {
            // Arrange
            using HttpClient client = this.factory.CreateClient();

            HttpResponseMessage response = null;

            // Act
            do
            {
                response?.Dispose();
                response = await client.GetAsync($"/TracingDemo/Hello");
            }
            while (response.IsSuccessStatusCode);

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType.MediaType);

            JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStreamAsync());
            JsonElement rootElement = document.RootElement;

            Assert.Equal("https://tools.ietf.org/html/rfc7231#section-6.6.4", rootElement.GetProperty("type").GetString());
            Assert.Equal("Service Unavailable", rootElement.GetProperty("title").GetString());
            Assert.Equal("This is intended 80% of the time. Please try again.", rootElement.GetProperty("detail").GetString());
            Assert.Equal(503, rootElement.GetProperty("status").GetInt32());

            response.Dispose();
        }
    }
}
