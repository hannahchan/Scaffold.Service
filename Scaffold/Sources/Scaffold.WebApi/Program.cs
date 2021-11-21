﻿namespace Scaffold.WebApi;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Scaffold.WebApi.Extensions;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build()
            .MigrateDatabase()
            .Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
