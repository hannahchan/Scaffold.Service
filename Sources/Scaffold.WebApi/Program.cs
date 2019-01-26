namespace Scaffold.WebApi
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Scaffold.WebApi.Extensions;

    public class Program
    {
        public static void Main(string[] args) =>
            CreateWebHostBuilder(args).Build()
                .MigrateDatabase()
                .Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
