namespace Scaffold.WebApi.IntegrationTests.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Scaffold.Repositories.PostgreSQL;
    using Xunit;

    public class BucketsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public BucketsControllerIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    ServiceDescriptor service = services.SingleOrDefault(service =>
                        service.ServiceType == typeof(DbContextOptions<BucketContext>));

                    if (service != null)
                    {
                        services.Remove(service);
                    }

                    services.AddDbContext<BucketContext>(options =>
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
                });
            });
        }

        [Fact]
        public async Task Test()
        {
            // Arrange
            System.Net.Http.HttpClient client = this.factory.CreateClient();

            // Act
            System.Net.Http.HttpResponseMessage response = await client.GetAsync("/health");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
