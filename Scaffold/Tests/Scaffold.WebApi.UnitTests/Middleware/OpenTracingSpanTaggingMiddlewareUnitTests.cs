namespace Scaffold.WebApi.UnitTests.Middleware
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using OpenTracing.Mock;
    using Scaffold.WebApi.Middleware;
    using Xunit;

    public class OpenTracingSpanTaggingMiddlewareUnitTests
    {
        [Theory]
        [InlineData(499, false)]
        [InlineData(500, true)]
        [InlineData(501, true)]
        public async Task When_InvokingMiddlewareWithStatusCode_Expect_SetTag(int statusCode, bool expectedError)
        {
            // Arrange
            MockTracer mockTracer = new MockTracer();

            OpenTracingSpanTaggingMiddleware middleware = new OpenTracingSpanTaggingMiddleware(
                (httpContext) => Task.CompletedTask,
                mockTracer);

            HttpContext context = new DefaultHttpContext();
            context.Response.StatusCode = statusCode;

            // Act
            using (mockTracer.BuildSpan("SomeWork").StartActive())
            {
                await middleware.Invoke(context);
            }

            // Assert
            MockSpan mockSpan = Assert.Single(mockTracer.FinishedSpans());

            if (mockSpan.Tags.ContainsKey("error"))
            {
                Assert.Equal(expectedError, mockSpan.Tags["error"]);
            }

            Assert.Equal(expectedError, mockSpan.Tags.ContainsKey("error"));
        }
    }
}
