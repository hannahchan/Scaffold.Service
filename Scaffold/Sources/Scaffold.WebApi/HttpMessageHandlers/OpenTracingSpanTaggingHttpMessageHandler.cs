namespace Scaffold.WebApi.HttpMessageHandlers
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using OpenTracing;
    using OpenTracing.Tag;

    public class OpenTracingSpanTaggingHttpMessageHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public OpenTracingSpanTaggingHttpMessageHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpContext httpContext = this.httpContextAccessor.HttpContext ??
                throw new InvalidOperationException("Missing HttpContext while processing request.");

            ITracer tracer = httpContext.RequestServices.GetRequiredService<ITracer>();

            try
            {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode && tracer.ActiveSpan is ISpan activeSpan)
                {
                    Tags.Error.Set(activeSpan, true);
                }

                return response;
            }
            catch
            {
                if (tracer.ActiveSpan is ISpan activeSpan)
                {
                    Tags.Error.Set(activeSpan, true);
                }

                throw;
            }
        }
    }
}
