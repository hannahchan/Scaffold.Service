namespace Scaffold.WebApi.IntegrationTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Xunit;

    public class MetricsEndpointIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public MetricsEndpointIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder => builder.ConfigureWithDefaultsForTesting());
        }

        [Fact]
        public async Task When_FilteringByPort_Expect_OkOnMetricsPort()
        {
            // Arrange
            int metricsPort = new Random().Next(1024, 65535);

            HttpClient client = this.factory.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("METRICSPORT", metricsPort.ToString());
                builder.UseSetting("URLS", $"http://+:80;http://+:{metricsPort}");
            }).CreateClient();

            // Act
            HttpResponseMessage expectedNotFoundResponse = await client.GetAsync("http://localhost/metrics");
            HttpResponseMessage expectedOkResponse = await client.GetAsync($"http://localhost:{metricsPort}/metrics");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, expectedNotFoundResponse.StatusCode);

            Assert.Equal(HttpStatusCode.OK, expectedOkResponse.StatusCode);
            Assert.Equal(MediaTypeNames.Text.Plain, expectedOkResponse.Content.Headers.ContentType.MediaType);
        }
    }
}
