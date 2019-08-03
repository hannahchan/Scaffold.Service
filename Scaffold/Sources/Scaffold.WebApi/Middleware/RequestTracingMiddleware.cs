namespace Scaffold.WebApi.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Scaffold.WebApi.Constants;
    using Scaffold.WebApi.Services;

    public class RequestTracingMiddleware
    {
        private readonly RequestDelegate next;

        public RequestTracingMiddleware(RequestDelegate next) => this.next = next;

        public async Task InvokeAsync(HttpContext context, RequestTracingService tracingService)
        {
            if (tracingService.CorrelationId != null)
            {
                throw new InvalidOperationException(
                    $"Make sure {nameof(RequestTracingService)} has been registered as a 'Scoped' service.");
            }

            // Attempt to read the Correlation ID from the request.
            tracingService.CorrelationId = context.Request.Headers[CustomHeaderNames.CorrelationId];

            // Use the Request ID as the Correlation ID if no Correlation ID exists.
            tracingService.CorrelationId = tracingService.CorrelationId ?? context.TraceIdentifier;

            // Add request tracing headers to response.
            context.Response.Headers.Add(CustomHeaderNames.CorrelationId, tracingService.CorrelationId);
            context.Response.Headers.Add(CustomHeaderNames.RequestId, context.TraceIdentifier);

            await this.next(context);
        }
    }
}
