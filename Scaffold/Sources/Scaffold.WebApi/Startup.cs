namespace Scaffold.WebApi
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Scaffold.Repositories.EntityFrameworkCore;
    using Scaffold.WebApi.Extensions;
    using Scaffold.WebApi.Filters;
    using Scaffold.WebApi.Middleware;

    public class Startup
    {
        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<BucketContext>();

            services
                .AddHttpClients()
                .AddOptions(this.Configuration)
                .AddRepositories(this.Configuration)
                .AddServices();

            services.AddControllers(options => options.Filters.Add<ExceptionFilter>())
                .AddCustomJsonOptions()
                .AddCustomXmlFormatters();

            services.AddApiDocumentation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app
                .UseForwardedHeaders()
                .UseMiddleware<RequestLoggingMiddleware>()
                .UseMiddleware<RequestTracingMiddleware>()
                .UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Scaffold.WebApi v1");
                });

            app
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("/health")
                        .RequireHost($"*:{this.Configuration["HealthCheckPort"]}");

                    endpoints.MapControllers();
                });
        }
    }
}
