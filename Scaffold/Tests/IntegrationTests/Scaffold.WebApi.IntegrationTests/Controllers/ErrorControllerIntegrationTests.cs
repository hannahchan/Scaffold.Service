namespace Scaffold.WebApi.IntegrationTests.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Net.Http.Headers;
    using Xunit;

    public class ErrorControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public ErrorControllerIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder => builder.ConfigureWithDefaultsForTesting());
        }

        [Fact]
        public async Task When_InvokingError_Expect_ProblemDetailsJsonResponse()
        {
            // Arrange
            HttpClient client = this.factory.CreateClient();
            client.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);

            // Act
            HttpResponseMessage response = await client.GetAsync("/Error");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType.MediaType);

            JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStreamAsync());

            Assert.True(document.RootElement.TryGetProperty("type", out _));
            Assert.True(document.RootElement.TryGetProperty("title", out _));
            Assert.True(document.RootElement.TryGetProperty("status", out _));
        }

        [Fact]
        public async Task When_InvokingError_Expect_ProblemDetailsXmlResponse()
        {
            // Arrange
            HttpClient client = this.factory.CreateClient();
            client.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Xml);

            // Act
            HttpResponseMessage response = await client.GetAsync("/Error");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(CustomMediaTypeNames.Application.ProblemXml, response.Content.Headers.ContentType.MediaType);

            XmlDocument document = new XmlDocument();
            document.LoadXml(await response.Content.ReadAsStringAsync());

            Assert.Equal("problem", document.DocumentElement.Name);
            Assert.Equal("status", document.DocumentElement.ChildNodes[0].Name);
            Assert.Equal("title", document.DocumentElement.ChildNodes[1].Name);
            Assert.Equal("type", document.DocumentElement.ChildNodes[2].Name);
        }
    }
}
