namespace Scaffold.WebApi.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Scaffold.Application.Interfaces;
    using Scaffold.WebApi.Constants;

    public class RequestIdMiddleware
    {
        private readonly RequestDelegate next;

        public RequestIdMiddleware(RequestDelegate next) => this.next = next;

        public async Task InvokeAsync(HttpContext context, IRequestIdService service)
        {
            if (service.RequestId != null)
            {
                throw new InvalidOperationException();
            }

            service.RequestId = context.Request.Headers[Headers.RequestId];
            context.Response.Headers.Add(Headers.RequestId, service.RequestId);

            await this.next(context);
        }
    }
}
