namespace Scaffold.WebApi.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Scaffold.WebApi.Middleware;

    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder app)
        {
            app
                .UseMiddleware<UnhandledExceptionHandler>()
                .UseHealthChecks("/health");

            return app;
        }
    }
}
