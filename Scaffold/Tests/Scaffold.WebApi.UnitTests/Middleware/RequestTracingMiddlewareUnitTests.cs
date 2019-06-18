namespace Scaffold.WebApi.UnitTests.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Scaffold.WebApi.Constants;
    using Scaffold.WebApi.Middleware;
    using Scaffold.WebApi.Services;
    using Xunit;

    public class RequestTracingMiddlewareUnitTests
    {
        [Fact]
        public async Task When_InvokingMiddlewareWithTracingServiceWithCorrelationId_Expect_InvalidOperationException()
        {
            // Arrange
            RequestTracingMiddleware middleware = new RequestTracingMiddleware((httpContext) => Task.CompletedTask);

            HttpContext context = new DefaultHttpContext();
            RequestTracingService tracingService = new RequestTracingService { CorrelationId = Guid.NewGuid().ToString() };

            // Act
            Exception exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(context, tracingService));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public async Task When_InvokingMiddlewareWithCorrelationIdInRequestHeader_Expect_CorrelationIdCopiedToTracingService()
        {
            // Arrange
            RequestTracingMiddleware middleware = new RequestTracingMiddleware((httpContext) => Task.CompletedTask);

            string correlationId = Guid.NewGuid().ToString();

            HttpContext context = new DefaultHttpContext();
            context.Request.Headers.Add(Headers.CorrelationId, correlationId);

            RequestTracingService tracingService = new RequestTracingService();

            // Act
            await middleware.InvokeAsync(context, tracingService);

            // Assert
            Assert.Equal(correlationId, tracingService.CorrelationId);
            Assert.Equal(correlationId, context.Response.Headers[Headers.CorrelationId]);
            Assert.Equal(context.TraceIdentifier, context.Response.Headers[Headers.RequestId]);
        }

        [Fact]
        public async Task When_InvokingMiddlewareWithNoCorrelationIdInRequestHeader_Expect_TraceIdentifierCopiedToTractingService()
        {
            // Arrange
            RequestTracingMiddleware middleware = new RequestTracingMiddleware((httpContext) => Task.CompletedTask);

            HttpContext context = new DefaultHttpContext();
            RequestTracingService tracingService = new RequestTracingService();

            // Act
            await middleware.InvokeAsync(context, tracingService);

            // Assert
            Assert.Equal(context.TraceIdentifier, tracingService.CorrelationId);
            Assert.Equal(context.TraceIdentifier, context.Response.Headers[Headers.CorrelationId]);
            Assert.Equal(context.TraceIdentifier, context.Response.Headers[Headers.RequestId]);
        }
    }
}
