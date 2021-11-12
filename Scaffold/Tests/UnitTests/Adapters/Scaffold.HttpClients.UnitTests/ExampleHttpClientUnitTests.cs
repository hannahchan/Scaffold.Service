namespace Scaffold.HttpClients.UnitTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class ExampleHttpClientUnitTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("test")]
        [InlineData("/test")]
        public async Task When_InvokingGetWithPath_Expect_RequestUri(string path)
        {
            // Arrange
            string content = Guid.NewGuid().ToString();

            HttpResponseMessage response = new HttpResponseMessage
            {
                Content = new StringContent(content),
                StatusCode = HttpStatusCode.OK,
            };

            Mock.HttpRequestHandler httpRequestHandler = new Mock.HttpRequestHandler(response);

            HttpResponseMessage result;

            // Act
            using (HttpClient httpClient = new HttpClient(httpRequestHandler))
            {
                ExampleHttpClient exampleHttpClient = new ExampleHttpClient(httpClient);
                result = await exampleHttpClient.Get(path);
            }

            // Assert
            Assert.Equal(HttpMethod.Get, result.RequestMessage.Method);
            Assert.Equal($"https://worldtimeapi.org/{(path ?? string.Empty).TrimStart('/')}", result.RequestMessage.RequestUri.ToString());
            Assert.Equal(content, await result.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task When_InvokingGetAndCancellationIsRequested_Expect_TaskCanceledException()
        {
            // Arrange
            Mock.HttpRequestHandler httpRequestHandler = new Mock.HttpRequestHandler(new HttpResponseMessage());

            Exception exception;

            // Act
            using (HttpClient httpClient = new HttpClient(httpRequestHandler))
            {
                ExampleHttpClient exampleHttpClient = new ExampleHttpClient(httpClient);
                exception = await Record.ExceptionAsync(() => exampleHttpClient.Get(string.Empty, new CancellationToken(true)));
            }

            // Assert
            Assert.IsType<TaskCanceledException>(exception);
        }
    }
}
