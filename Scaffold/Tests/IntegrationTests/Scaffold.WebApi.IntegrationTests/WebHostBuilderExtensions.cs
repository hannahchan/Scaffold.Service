namespace Scaffold.WebApi.IntegrationTests;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scaffold.Repositories;

internal static class WebHostBuilderExtensions
{
    private static readonly InMemoryDatabaseRoot DatabaseRoot = new InMemoryDatabaseRoot();

    public static IWebHostBuilder ConfigureWithDefaultsForTesting(this IWebHostBuilder builder)
    {
        return builder
            .ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
                {
                });
            })
            .ConfigureLogging(logging => logging.ClearProviders().AddDebug())
            .UseSetting("Environment", "Production");
    }

    public static IWebHostBuilder ConfigureInMemoryDatabase(this IWebHostBuilder builder)
    {
        string databaseName = Guid.NewGuid().ToString();

        return builder
            .ConfigureServices(services =>
            {
                services.Remove(services.SingleOrDefault(service =>
                    service.ServiceType == typeof(DbContextOptions<BucketContext>)));

                services.AddDbContext<BucketContext>(options =>
                    options.UseInMemoryDatabase(databaseName, DatabaseRoot));

                services.Remove(services.SingleOrDefault(service =>
                    service.ServiceType == typeof(DbContextOptions<BucketContext.ReadOnly>)));

                services.AddDbContext<BucketContext.ReadOnly>(options =>
                    options.UseInMemoryDatabase(databaseName, DatabaseRoot));
            });
    }
}
