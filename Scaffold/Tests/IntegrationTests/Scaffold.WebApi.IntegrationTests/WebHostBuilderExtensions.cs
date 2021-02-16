namespace Scaffold.WebApi.IntegrationTests
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigureWithDefaultsForTesting(this IWebHostBuilder builder)
        {
            return builder
                .ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
                {
                    configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "Jaeger:AgentHost", "localhost" },
                    });
                })
                .ConfigureLogging(logging => logging.ClearProviders().AddDebug());
        }
    }
}
