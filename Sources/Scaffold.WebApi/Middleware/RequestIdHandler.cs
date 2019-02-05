namespace Scaffold.WebApi.Middleware
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class RequestIdHandler
    {
        private readonly RequestDelegate next;

        public RequestIdHandler(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await this.next(context);
        }
    }
}
