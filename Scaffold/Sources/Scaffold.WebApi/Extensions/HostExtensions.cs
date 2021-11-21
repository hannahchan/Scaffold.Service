namespace Scaffold.WebApi.Extensions;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scaffold.Repositories;

internal static class HostExtensions
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using (IServiceScope serviceScope = host.Services.CreateScope())
        {
            IServiceProvider serviceProvider = serviceScope.ServiceProvider;
            IHostEnvironment hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();

            if (hostEnvironment.IsDevelopment())
            {
                BucketContext context = serviceProvider.GetRequiredService<BucketContext>();
                context.Database.Migrate();
            }
        }

        return host;
    }
}
