namespace Scaffold.WebApi;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scaffold.Repositories;
using Scaffold.WebApi.Extensions;
using Scaffold.WebApi.Filters;
using Scaffold.WebApi.Middleware;

public class Program
{
    protected Program()
    {
    }

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ConfigureWebApplicationBuilder(builder);

        using WebApplication app = builder.Build();
        ConfigureWebApplication(app);

        if (app.Environment.IsDevelopment())
        {
            app.MigrateDatabase();
        }

        app.Run();
    }

    // Configures OpenTelemetry and adds services to the container.
    private static void ConfigureWebApplicationBuilder(WebApplicationBuilder builder)
    {
        builder.AddOpenTelemetry();

        builder.Services.AddCors(options => options.AddDefaultPolicy(builder =>
        {
            // Not recommended for production. Please remove or revise for your environment.
            builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        }));

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<BucketContext>()
            .AddDbContextCheck<BucketContext.ReadOnly>();

        builder.Services
            .AddHttpClients()
            .AddOptions(builder.Configuration)
            .AddRepositories(builder.Configuration)
            .AddServices();

        builder.Services.AddControllers(options => options.Filters.Add<ExceptionFilter>())
            .AddCustomJsonOptions()
            .AddCustomXmlFormatters();

        builder.Services.AddApiDocumentation();
    }

    // Configures the HTTP request pipeline.
    // More information at https://docs.microsoft.com/aspnet/core/fundamentals/middleware
    private static void ConfigureWebApplication(WebApplication app)
    {
        // Configures middleware
        app
            .UseExceptionHandler("/error")
            .UseMiddleware<RequestLoggingMiddleware>()
            .UseSwagger()
            .UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Scaffold.WebApi v1"))
            .UseCors()
            .UseAuthorization();

        // Configures endpoints
        IEndpointConventionBuilder healthCheckEndpoint = app.MapHealthChecks("/health");

        if (int.TryParse(app.Configuration["HealthCheckPort"], out int healthCheckPort))
        {
            healthCheckEndpoint.RequireHost($"*:{healthCheckPort}");
        }

        IEndpointConventionBuilder controllerEndpoints = app.MapControllers();

        if (int.TryParse(app.Configuration["DefaultPort"], out int defaultPort))
        {
            controllerEndpoints.RequireHost($"*:{defaultPort}");
        }
    }
}
