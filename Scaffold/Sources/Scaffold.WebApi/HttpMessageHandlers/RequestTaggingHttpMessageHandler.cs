namespace Scaffold.WebApi.HttpMessageHandlers
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Net.Http.Headers;
    using Scaffold.WebApi.Constants;
    using Scaffold.WebApi.Services;

    public class RequestTaggingHttpMessageHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public RequestTaggingHttpMessageHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IServiceProvider serviceProvider = this.httpContextAccessor.HttpContext.RequestServices;

            RequestTracingService tracingService = serviceProvider.GetRequiredService<RequestTracingService>();
            request.Headers.Add(CustomHeaderNames.CorrelationId, tracingService.CorrelationId);

            IWebHostEnvironment webHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            request.Headers.Add(HeaderNames.UserAgent, webHostEnvironment.ApplicationName);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
