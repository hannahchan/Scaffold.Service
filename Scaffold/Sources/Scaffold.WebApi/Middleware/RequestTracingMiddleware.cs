namespace Scaffold.WebApi.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Scaffold.Application.Interfaces;
    using Scaffold.WebApi.Constants;

    public class RequestTracingMiddleware
    {
        private readonly RequestDelegate next;

        public RequestTracingMiddleware(RequestDelegate next) => this.next = next;

        public async Task InvokeAsync(HttpContext context, IRequestTracingService tracingService)
        {
            if (tracingService.CorrelationId != null)
            {
                throw new InvalidOperationException(
                    $"Make sure {nameof(IRequestTracingService)} has been registered as a 'Scoped' service.");
            }

            // Attempt to read the Correlation ID from the request.
            tracingService.CorrelationId = context.Request.Headers[Headers.CorrelationId];

            // Use the Request ID as the Correlation ID if no Correlation ID exists.
            tracingService.CorrelationId = tracingService.CorrelationId ?? context.TraceIdentifier;

            // Add request tracing headers to response.
            context.Response.Headers.Add(Headers.CorrelationId, tracingService.CorrelationId);
            context.Response.Headers.Add(Headers.RequestId, context.TraceIdentifier);

            await this.next(context);
        }
    }
}
