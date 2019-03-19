namespace Scaffold.WebApi.Extensions
{
    using System;
    using System.IO;
    using System.Reflection;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.Application.Interfaces;
    using Scaffold.Data;
    using Scaffold.Data.Repositories;
    using Scaffold.WebApi.Services;

    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                OpenApiInfo info = new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Scaffold",
                    Description = "A simple CRUD application to demonstrate Scaffold."
                };

                options.SwaggerDoc("v1", info);

                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            return services;
        }

        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<BucketContext>(builder =>
                builder.UseNpgsql(config.GetValue<string>("ConnectionStrings:DefaultConnection")));

            return services;
        }

        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration config)
        {
            services
                .Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.All);

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services
                .AddScoped<IBucketRepository, BucketRepository>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddScoped<IRequestTracingService, RequestTracingService>();

            return services;
        }

        public static IServiceCollection AddUtilities(this IServiceCollection services)
        {
            services
                .AddAutoMapper()
                .AddMediatR(typeof(GetBucket).Assembly);

            return services;
        }
    }
}
