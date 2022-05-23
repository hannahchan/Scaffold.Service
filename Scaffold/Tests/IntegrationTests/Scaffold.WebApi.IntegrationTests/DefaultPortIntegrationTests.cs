namespace Scaffold.WebApi.IntegrationTests;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class DefaultPortIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public DefaultPortIntegrationTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory.WithWebHostBuilder(builder => builder
            .ConfigureWithDefaultsForTesting()
            .ConfigureInMemoryDatabase());
    }

    [Fact]
    public async Task When_FilteringByDefaultPort_Expect_OkOnDefaultPort()
    {
        // Arrange
        int defaultPort = new Random().Next(1024, 65535);

        using HttpClient client = this.factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("DEFAULTPORT", defaultPort.ToString());
            builder.UseSetting("URLS", $"http://+:80;http://+:{defaultPort}");
        }).CreateClient();

        // Act
        using HttpResponseMessage expectedNotFoundResponse = await client.GetAsync("http://localhost/buckets");
        using HttpResponseMessage expectedOkResponse = await client.GetAsync($"http://localhost:{defaultPort}/buckets");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, expectedNotFoundResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, expectedOkResponse.StatusCode);
    }

    [Fact]
    public async Task When_DefaultPortIsNull_Expect_OkOnAllPort()
    {
        // Arrange
        int randomPort1 = new Random().Next(1024, 65535);
        int randomPort2 = new Random().Next(1024, 65535);

        using HttpClient client = this.factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("DEFAULTPORT", null);
            builder.UseSetting("URLS", $"http://+:{randomPort1};http://+:{randomPort2}");
        }).CreateClient();

        // Act
        using HttpResponseMessage expectedOkResponse1 = await client.GetAsync($"http://localhost:{randomPort1}/buckets");
        using HttpResponseMessage expectedOkResponse2 = await client.GetAsync($"http://localhost:{randomPort2}/buckets");

        // Assert
        Assert.Equal(HttpStatusCode.OK, expectedOkResponse1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, expectedOkResponse2.StatusCode);
    }
}
