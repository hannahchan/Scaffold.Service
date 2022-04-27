namespace Scaffold.WebApi.Extensions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Prometheus;
using Scaffold.Application.Common.Constants;
using Scaffold.Application.Common.Interfaces;
using Scaffold.Application.Components.Bucket;
using Scaffold.HttpClients;
using Scaffold.Repositories;
using Scaffold.WebApi.Controllers;
using Scaffold.WebApi.Middleware;
using Swashbuckle.AspNetCore.SwaggerGen;

internal static class ServiceCollectionExtensions
{
    private static readonly string[] IgnorePatterns =
    {
        "^/health$",
        "^/metrics$",
    };

    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            OpenApiInfo info = new OpenApiInfo
            {
                Version = "v1",
                Title = "Scaffold",
                Description = "A simple CRUD application to demonstrate Scaffold.",
            };

            options.SwaggerDoc("v1", info);

            options.CustomOperationIds(apiDescription =>
                apiDescription.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null);

            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IExampleHttpClient, ExampleHttpClient>()
            .AddTransientHttpErrorPolicy(builder => builder.RetryAsync(3))
            .AddRequestLogging()
            .UseHttpClientMetrics();

        services.AddHttpClient<DemoController.IClient, DemoController.Client>()
            .AddTransientHttpErrorPolicy(builder => builder.RetryAsync(3))
            .AddRequestLogging()
            .UseHttpClientMetrics();

        return services;
    }

    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration config)
    {
        services.AddOpenTelemetryTracing(builder =>
        {
            // Configure OpenTelemetry
            string serviceName = config.GetValue<string>("OpenTelemetry:ServiceName");
            string serviceNamespace = config.GetValue<string>("OpenTelemetry:ServiceNamespace");

            ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(serviceName, serviceNamespace);

            builder.SetResourceBuilder(resourceBuilder);

            // Add Instrumentation / Sources
            builder
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = httpContext => !httpContext.Request.Path.IgnorePath(IgnorePatterns);
                })
                .AddHttpClientInstrumentation()
                .AddNpgsql()
                .AddSource(Application.ActivitySource.Name);

            // Add Exporters
            builder.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(config.GetValue<string>("OpenTelemetry:Endpoint"));
                options.Protocol = OtlpExportProtocol.Grpc;
            });
        });

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration config)
    {
        services
            .Configure<RequestLoggingMiddleware.Options>(options => options.IgnorePatterns = IgnorePatterns)
            .Configure<DemoController.Options>(config.GetSection("DemoOptions"));

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<BucketContext>(builder =>
            builder.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddDbContext<BucketContext.ReadOnly>(builder =>
            builder.UseNpgsql(config.GetConnectionString("ReadOnlyConnection")));

        services
            .AddScoped<IBucketReadRepository, ScopedBucketReadRepository>()
            .AddScoped<IBucketRepository, ScopedBucketRepository>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddAutoMapper(typeof(Startup).Assembly)
            .AddMediatR(typeof(Application).Assembly);

        return services;
    }

    private static bool IgnorePath(this PathString path, IEnumerable<string> ignorePatterns)
    {
        return ignorePatterns.Any(ignorePattern => Regex.IsMatch(path, ignorePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase));
    }
}
