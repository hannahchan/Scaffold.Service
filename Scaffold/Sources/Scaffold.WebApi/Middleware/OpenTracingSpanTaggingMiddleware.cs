namespace Scaffold.WebApi.Middleware
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using OpenTracing;
    using OpenTracing.Tag;

    public class OpenTracingSpanTaggingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly ITracer tracer;

        public OpenTracingSpanTaggingMiddleware(RequestDelegate next, ITracer tracer)
        {
            this.next = next;
            this.tracer = tracer;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await this.next.Invoke(httpContext);

            if (httpContext.Response.StatusCode >= 500)
            {
                Tags.Error.Set(this.tracer.ActiveSpan, true);
            }
        }
    }
}
