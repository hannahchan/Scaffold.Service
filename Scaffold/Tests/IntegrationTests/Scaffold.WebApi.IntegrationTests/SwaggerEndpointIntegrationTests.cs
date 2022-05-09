namespace Scaffold.WebApi.IntegrationTests;

using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class SwaggerEndpointIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public SwaggerEndpointIntegrationTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory.WithWebHostBuilder(builder => builder.ConfigureWithDefaultsForTesting());
    }

    [Fact]
    public async Task When_FetchingSwaggerDocument_Expect_Ok()
    {
        // Arrange
        using HttpClient client = this.factory.CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync("http://localhost/swagger/v1/swagger.json");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType.MediaType);
    }

    [Fact]
    public async Task When_FetchingSwaggerUI_Expect_Ok()
    {
        // Arrange
        using HttpClient client = this.factory.CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync("http://localhost/swagger");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Html, response.Content.Headers.ContentType.MediaType);
    }
}
