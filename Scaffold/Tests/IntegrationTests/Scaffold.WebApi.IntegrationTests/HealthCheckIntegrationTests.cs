namespace Scaffold.WebApi.IntegrationTests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Repositories;
using Xunit;

public class HealthCheckIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public HealthCheckIntegrationTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureWithDefaultsForTesting();

            // Override Connection Strings - Start off with unreachable database hosts
            int invalidDbPort = new Random().Next(10000, 65535);

            builder.ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:DefaultConnection", $"Host=localhost;Port={invalidDbPort};" },
                    { "ConnectionStrings:ReadOnlyConnection", $"Host=localhost;Port={invalidDbPort};" },
                }));
        });
    }

    [Fact]
    public async Task When_AllDatabaseConnectionsAreAvailable_Expect_Ok()
    {
        // Arrange
        using HttpClient client = this.factory.WithWebHostBuilder(builder =>
        {
            builder
                .ConfigureInMemoryDatabase()
                .UseSetting("HEALTHCHECKPORT", "80");
        }).CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.MediaType);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task When_AllDatabaseConnectionsAreUnavailable_Expect_ServiceUnavailable()
    {
        // Arrange
        using HttpClient client = this.factory
            .WithWebHostBuilder(builder => builder.UseSetting("HEALTHCHECKPORT", "80"))
            .CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.MediaType);
        Assert.Equal("Unhealthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task When_ReadWriteDatabaseConnectionIsUnavailable_Expect_ServiceUnavailable()
    {
        // Arrange
        using HttpClient client = this.factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.Remove(services.SingleOrDefault(service =>
                    service.ServiceType == typeof(DbContextOptions<BucketContext.ReadOnly>)));

                services.AddDbContext<BucketContext.ReadOnly>(options =>
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            });

            builder.UseSetting("HEALTHCHECKPORT", "80");
        }).CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.MediaType);
        Assert.Equal("Unhealthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task When_ReadOnlyDatabaseConnectionIsUnavailable_Expect_ServiceUnavailable()
    {
        // Arrange
        using HttpClient client = this.factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.Remove(services.SingleOrDefault(service =>
                    service.ServiceType == typeof(DbContextOptions<BucketContext>)));

                services.AddDbContext<BucketContext>(options =>
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            });

            builder.UseSetting("HEALTHCHECKPORT", "80");
        }).CreateClient();

        // Act
        using HttpResponseMessage response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType.MediaType);
        Assert.Equal("Unhealthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task When_FilteringByPort_Expect_OkOnHealthCheckPort()
    {
        // Arrange
        int healthCheckPort = new Random().Next(1024, 65535);

        using HttpClient client = this.factory.WithWebHostBuilder(builder =>
        {
            builder
                .ConfigureInMemoryDatabase()
                .UseSetting("HEALTHCHECKPORT", healthCheckPort.ToString())
                .UseSetting("URLS", $"http://+:80;http://+:{healthCheckPort}");
        }).CreateClient();

        // Act
        using HttpResponseMessage expectedNotFoundResponse = await client.GetAsync("http://localhost/health");
        using HttpResponseMessage expectedOkResponse = await client.GetAsync($"http://localhost:{healthCheckPort}/health");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, expectedNotFoundResponse.StatusCode);

        Assert.Equal(HttpStatusCode.OK, expectedOkResponse.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, expectedOkResponse.Content.Headers.ContentType.MediaType);
        Assert.Equal("Healthy", await expectedOkResponse.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task When_HealthCheckPortIsNull_Expect_OkOnAllPort()
    {
        // Arrange
        int randomPort1 = new Random().Next(1024, 65535);
        int randomPort2 = new Random().Next(1024, 65535);

        using HttpClient client = this.factory.WithWebHostBuilder(builder =>
        {
            builder
                .ConfigureInMemoryDatabase()
                .UseSetting("HEALTHCHECKPORT", null)
                .UseSetting("URLS", $"http://+:{randomPort1};http://+:{randomPort2}");
        }).CreateClient();

        // Act
        using HttpResponseMessage expectedOkResponse1 = await client.GetAsync($"http://localhost:{randomPort1}/health");
        using HttpResponseMessage expectedOkResponse2 = await client.GetAsync($"http://localhost:{randomPort2}/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, expectedOkResponse1.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, expectedOkResponse1.Content.Headers.ContentType.MediaType);
        Assert.Equal("Healthy", await expectedOkResponse1.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, expectedOkResponse2.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, expectedOkResponse2.Content.Headers.ContentType.MediaType);
        Assert.Equal("Healthy", await expectedOkResponse2.Content.ReadAsStringAsync());
    }
}
