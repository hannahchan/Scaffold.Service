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
            IServiceProvider serviceProvider = this.httpContextAccessor.HttpContext.RequestServices;
            ITracer tracer = serviceProvider.GetRequiredService<ITracer>();

            try
            {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

                if ((int)response.StatusCode >= 500)
                {
                    Tags.Error.Set(tracer.ActiveSpan, true);
                }

                return response;
            }
            catch
            {
                Tags.Error.Set(tracer.ActiveSpan, true);

                throw;
            }
        }
    }
}
