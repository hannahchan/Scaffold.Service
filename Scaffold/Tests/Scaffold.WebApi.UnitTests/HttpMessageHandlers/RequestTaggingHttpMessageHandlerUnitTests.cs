namespace Scaffold.WebApi.UnitTests.HttpMessageHandlers
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Scaffold.WebApi.HttpMessageHandlers;
    using Scaffold.WebApi.Services;
    using Xunit;

    public class RequestTaggingHttpMessageHandlerUnitTests
    {
        [Fact]
        public async Task When_SendingAsync_Expect_HeadersSetOnRequest()
        {
            // Arrange
            string applicationName = Guid.NewGuid().ToString();
            string correlationId = Guid.NewGuid().ToString();

            ServiceCollection services = new ServiceCollection();
            services.AddScoped<IWebHostEnvironment, TestWebHostEnvironment>();
            services.AddScoped<RequestTracingService>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IWebHostEnvironment webHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            webHostEnvironment.ApplicationName = applicationName;

            RequestTracingService tracingService = serviceProvider.GetRequiredService<RequestTracingService>();
            tracingService.CorrelationId = correlationId;

            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext { RequestServices = serviceProvider },
            };

            RequestTaggingHttpMessageHandler handler = new RequestTaggingHttpMessageHandler(httpContextAccessor)
            {
                InnerHandler = new InnerHandler(),
            };

            HttpRequestHeaders result;

            // Act
            using (HttpClient client = new HttpClient(handler))
            {
                HttpResponseMessage response = await client.GetAsync(new Uri("http://localhost"));
                result = response.RequestMessage.Headers;
            }

            // Assert
            Assert.True(result.Contains("User-Agent"));
            Assert.Contains(applicationName, result.GetValues("User-Agent"));
            Assert.True(result.Contains("Correlation-Id"));
            Assert.Contains(correlationId, result.GetValues("Correlation-Id"));
        }

        private class InnerHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return await Task.FromResult(new HttpResponseMessage { RequestMessage = request });
            }
        }

        private class TestWebHostEnvironment : IWebHostEnvironment
        {
            public string EnvironmentName { get; set; } = null!;

            public string ApplicationName { get; set; } = null!;

            public string WebRootPath { get; set; } = null!;

            public IFileProvider WebRootFileProvider { get; set; } = null!;

            public string ContentRootPath { get; set; } = null!;

            public IFileProvider ContentRootFileProvider { get; set; } = null!;
        }
    }
}
