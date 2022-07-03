namespace Scaffold.WebApi.Extensions;

using System;
using System.IO;
using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Polly;
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
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

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
            .AddRequestLogging();

        services.AddHttpClient<DemoController.IClient, DemoController.Client>()
            .AddTransientHttpErrorPolicy(builder => builder.RetryAsync(3))
            .AddRequestLogging();

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration config)
    {
        services
            .Configure<RequestLoggingMiddleware.Options>(options => options.IgnorePatterns = new[] { "^/health$", "^/metrics$" })
            .Configure<DemoController.Options>(config.GetSection("DemoOptions"))
            .Configure<DemoController.Options>(options =>
            {
                if (int.TryParse(config["DefaultPort"], out int defaultPort))
                {
                    options.NextHopBaseAddress ??= $"http://localhost:{defaultPort}";
                }

                options.NextHopBaseAddress ??= "http://localhost";
            });

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
            .AddAutoMapper(typeof(Program).Assembly)
            .AddMediatR(typeof(Application).Assembly);

        return services;
    }
}
