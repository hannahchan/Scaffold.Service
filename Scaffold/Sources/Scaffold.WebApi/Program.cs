namespace Scaffold.WebApi
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Scaffold.WebApi.Extensions;
    using Serilog;

    public static class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args).Build()
                .MigrateDatabase()
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    webBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
                        loggerConfiguration
                            .ReadFrom.Configuration(hostingContext.Configuration)
                            .Enrich.FromLogContext()
                            .WriteTo.Console());
                });
    }
}
