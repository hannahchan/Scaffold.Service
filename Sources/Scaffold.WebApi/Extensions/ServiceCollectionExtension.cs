namespace Scaffold.WebApi.Extensions
{
    using AutoMapper;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Scaffold.Application.Behavior;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.Application.Repositories;
    using Scaffold.Data;

    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<BucketContext>(builder =>
                builder.UseNpgsql(config.GetValue<string>("ConnectionStrings:DefaultConnection")));

            return services;
        }

        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration config)
        {
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBucketRepository, BucketRepository>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestIdBehavior<,>));

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
