namespace Scaffold.WebApi
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Scaffold.WebApi.Extensions;
    using Serilog;

    public class Program
    {
        public static void Main(string[] args) =>
            CreateWebHostBuilder(args).Build()
                .MigrateDatabase()
                .Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console();
                });
    }
}
