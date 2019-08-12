namespace Scaffold.WebApi.Extensions
{
    using System;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Scaffold.Repositories.EntityFrameworkCore;

    public static class WebHostExtension
    {
        public static IWebHost EnsureCreatedDatabase(this IWebHost webHost)
        {
            if (webHost == null)
            {
                throw new ArgumentNullException(nameof(webHost));
            }

            using (IServiceScope serviceScope = webHost.Services.CreateScope())
            {
                IServiceProvider serviceProvider = serviceScope.ServiceProvider;
                IHostingEnvironment hostingEnvironment = serviceProvider.GetRequiredService<IHostingEnvironment>();

                if (hostingEnvironment.IsDevelopment())
                {
                    BucketContext context = serviceProvider.GetService<BucketContext>();
                    context?.Database.EnsureCreated();
                }
            }

            return webHost;
        }

        public static IWebHost MigrateDatabase(this IWebHost webHost)
        {
            if (webHost == null)
            {
                throw new ArgumentNullException(nameof(webHost));
            }

            using (IServiceScope serviceScope = webHost.Services.CreateScope())
            {
                IServiceProvider serviceProvider = serviceScope.ServiceProvider;
                IHostingEnvironment hostingEnvironment = serviceProvider.GetRequiredService<IHostingEnvironment>();

                if (hostingEnvironment.IsDevelopment())
                {
                    BucketContext context = serviceProvider.GetService<BucketContext>();
                    context?.Database.Migrate();
                }
            }

            return webHost;
        }
    }
}
