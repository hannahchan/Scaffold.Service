namespace Scaffold.WebApi.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Scaffold.WebApi.Middleware;

    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder app, IConfiguration config)
        {
            app
                .UseMiddleware<UnhandledExceptionMiddleware>()
                .UseHealthChecks("/health", config["HealthCheckPort"])
                .UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Scaffold.WebApi v1");
                })
                .UseMiddleware<RequestIdMiddleware>();

            return app;
        }
    }
}
