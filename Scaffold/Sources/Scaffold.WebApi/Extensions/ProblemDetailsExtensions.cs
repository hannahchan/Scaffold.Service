namespace Scaffold.WebApi.Extensions
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using OpenTracing;

    public static class ProblemDetailsExtensions
    {
        public static ProblemDetails AddOpenTracingTraceId(this ProblemDetails details, ITracer tracer)
        {
            if (details is null)
            {
                throw new ArgumentNullException(nameof(details));
            }

            if (tracer is null)
            {
                throw new ArgumentNullException(nameof(tracer));
            }

            if (tracer.ActiveSpan is ISpan span)
            {
                details.Extensions["traceId"] = span.Context.TraceId;
            }

            return details;
        }
    }
}
