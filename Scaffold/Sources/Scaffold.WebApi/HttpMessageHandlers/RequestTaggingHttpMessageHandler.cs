namespace Scaffold.WebApi.HttpMessageHandlers
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;
    using Scaffold.WebApi.Constants;
    using Scaffold.WebApi.Services;

    public class RequestTaggingHttpMessageHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public RequestTaggingHttpMessageHandler(IHttpContextAccessor httpContextAccessor) =>
            this.httpContextAccessor = httpContextAccessor;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IServiceProvider serviceProvider = this.httpContextAccessor.HttpContext.RequestServices;

            RequestTracingService tracingService = serviceProvider.GetService(typeof(RequestTracingService)) as RequestTracingService;
            request.Headers.Add(CustomHeaderNames.CorrelationId, tracingService.CorrelationId);

            IHostingEnvironment hostingEnvironment = serviceProvider.GetService(typeof(IHostingEnvironment)) as IHostingEnvironment;
            request.Headers.Add(HeaderNames.UserAgent, hostingEnvironment.ApplicationName);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
