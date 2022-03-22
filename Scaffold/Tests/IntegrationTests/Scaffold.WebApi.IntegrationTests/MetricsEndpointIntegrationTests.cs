namespace Scaffold.WebApi.IntegrationTests;

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

        using HttpClient client = this.factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("METRICSPORT", metricsPort.ToString());
            builder.UseSetting("URLS", $"http://+:80;http://+:{metricsPort}");
        }).CreateClient();

        // Act
        using HttpResponseMessage expectedNotFoundResponse = await client.GetAsync("http://localhost/metrics");
        using HttpResponseMessage expectedOkResponse = await client.GetAsync($"http://localhost:{metricsPort}/metrics");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, expectedNotFoundResponse.StatusCode);

        Assert.Equal(HttpStatusCode.OK, expectedOkResponse.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, expectedOkResponse.Content.Headers.ContentType.MediaType);
    }

    [Fact]
    public async Task When_MetricsPortIsNull_Expect_OkOnAllPort()
    {
        // Arrange
        int randomPort1 = new Random().Next(1024, 65535);
        int randomPort2 = new Random().Next(1024, 65535);

        using HttpClient client = this.factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("METRICSPORT", null);
            builder.UseSetting("URLS", $"http://+:{randomPort1};http://+:{randomPort2}");
        }).CreateClient();

        // Act
        using HttpResponseMessage expectedOkResponse1 = await client.GetAsync($"http://localhost:{randomPort1}/metrics");
        using HttpResponseMessage expectedOkResponse2 = await client.GetAsync($"http://localhost:{randomPort2}/metrics");

        // Assert
        Assert.Equal(HttpStatusCode.OK, expectedOkResponse1.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, expectedOkResponse1.Content.Headers.ContentType.MediaType);

        Assert.Equal(HttpStatusCode.OK, expectedOkResponse2.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, expectedOkResponse2.Content.Headers.ContentType.MediaType);
    }
}
