namespace Scaffold.WebApi.Extensions
{
    using System;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Scaffold.Repositories.EntityFrameworkCore;

    public static class HostExtension
    {
        public static IHost EnsureCreatedDatabase(this IHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            using (IServiceScope serviceScope = host.Services.CreateScope())
            {
                IServiceProvider serviceProvider = serviceScope.ServiceProvider;
                IHostEnvironment hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();

                if (hostEnvironment.IsDevelopment())
                {
                    BucketContext context = serviceProvider.GetService<BucketContext>();
                    context?.Database.EnsureCreated();
                }
            }

            return host;
        }

        public static IHost MigrateDatabase(this IHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            using (IServiceScope serviceScope = host.Services.CreateScope())
            {
                IServiceProvider serviceProvider = serviceScope.ServiceProvider;
                IHostEnvironment hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();

                if (hostEnvironment.IsDevelopment())
                {
                    BucketContext context = serviceProvider.GetService<BucketContext>();
                    context?.Database.Migrate();
                }
            }

            return host;
        }
    }
}
