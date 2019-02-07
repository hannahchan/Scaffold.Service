namespace Scaffold.WebApi.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Scaffold.Application.Interfaces;

    public class RequestIdHandler
    {
        private readonly RequestDelegate next;

        public RequestIdHandler(RequestDelegate next) => this.next = next;

        public async Task InvokeAsync(HttpContext context, IRequestIdService service)
        {
            if (service.RequestId != null)
            {
                throw new InvalidOperationException();
            }

            service.RequestId = Guid.NewGuid().ToString();

            context.Response.Headers.Add("Request-Id", service.RequestId);

            await this.next(context);
        }
    }
}
