namespace Scaffold.WebApi.Middleware
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;

    public class UnhandledExceptionHandler
    {
        private readonly RequestDelegate next;

        private readonly IHostingEnvironment env;

        public UnhandledExceptionHandler(RequestDelegate next, IHostingEnvironment env)
        {
            this.next = next;
            this.env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await this.next.Invoke(httpContext);
            }
            catch (Exception exception)
            {
                httpContext.Response.ContentType = "text/plain";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                if (this.env.IsDevelopment())
                {
                    await httpContext.Response.WriteAsync(exception.ToString());
                    return;
                }
            }
        }
    }
}
