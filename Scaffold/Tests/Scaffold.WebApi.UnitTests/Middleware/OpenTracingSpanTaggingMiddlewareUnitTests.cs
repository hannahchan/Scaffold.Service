namespace Scaffold.WebApi.UnitTests.Middleware
{
    using System;
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
            using (mockTracer.BuildSpan("Unit Test").StartActive())
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

        [Fact]
        public async Task When_InvokingMiddlewareWithException_Expect_SetTagError()
        {
            // Arrange
            Exception exception = new Exception("Unit Test");

            MockTracer mockTracer = new MockTracer();

            OpenTracingSpanTaggingMiddleware middleware = new OpenTracingSpanTaggingMiddleware(
                (httpContext) => throw exception,
                mockTracer);

            Exception result;

            // Act
            using (mockTracer.BuildSpan("Unit Test").StartActive())
            {
                result = await Record.ExceptionAsync(() => middleware.Invoke(new DefaultHttpContext()));
            }

            // Assert
            MockSpan mockSpan = Assert.Single(mockTracer.FinishedSpans());
            Assert.True(mockSpan.Tags.ContainsKey("error"));
            Assert.True(Assert.IsType<bool>(mockSpan.Tags["error"]));

            Assert.NotNull(result);
            Assert.Equal(exception, result);
        }
    }
}
