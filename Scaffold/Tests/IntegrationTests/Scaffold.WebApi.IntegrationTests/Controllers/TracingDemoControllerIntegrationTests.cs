namespace Scaffold.WebApi.IntegrationTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
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
                        .ConfigureLogging(logging => logging.ClearProviders())
                        .ConfigureServices(services =>
                            services.AddTransient<TracingDemoController.IClient, MockTracingDemoClient>());
                });
            }

            [Fact]
            public async Task When_SayingHelloToName_Expect_HelloMessage()
            {
                // Arrange
                HttpClient client = this.factory.CreateClient();
                string name = Guid.NewGuid().ToString();

                // Act
                HttpResponseMessage response = await client.GetAsync($"/TracingDemo?name={name}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.MediaType);
                Assert.Equal($"Hello, {name}!", await response.Content.ReadAsStringAsync());
            }

            [Fact]
            public async Task When_SayingHelloToNullName_Expect_HelloMessage()
            {
                // Arrange
                HttpClient client = this.factory.CreateClient();

                // Act
                HttpResponseMessage response = await client.GetAsync($"/TracingDemo");

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
                this.factory = factory.WithWebHostBuilder(builder =>
                    builder.ConfigureLogging(logging => logging.ClearProviders()));
            }

            [Fact]
            public async Task When_SayingHelloToName_Expect_HelloMessage()
            {
                // Arrange
                HttpClient client = this.factory.CreateClient();
                string name = Guid.NewGuid().ToString();

                // Act
                HttpResponseMessage response = await client.GetAsync($"/TracingDemo/Hello?name={name}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.MediaType);
                Assert.Equal($"Hello, {name}!", await response.Content.ReadAsStringAsync());
            }

            [Fact]
            public async Task When_SayingHelloToNullName_Expect_HelloMessage()
            {
                // Arrange
                HttpClient client = this.factory.CreateClient();

                // Act
                HttpResponseMessage response = await client.GetAsync($"/TracingDemo/Hello");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.MediaType);
                Assert.Equal($"Hello, random!", await response.Content.ReadAsStringAsync());
            }
        }
    }
}
