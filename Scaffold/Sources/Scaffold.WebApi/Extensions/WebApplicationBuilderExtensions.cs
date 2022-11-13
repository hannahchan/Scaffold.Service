namespace Scaffold.WebApi.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scaffold.Application.Common.Constants;

internal static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault();

        return builder
            .ConfigureLogging(resourceBuilder)
            .ConfigureMetrics(resourceBuilder)
            .ConfigureTracing(resourceBuilder);
    }

    private static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder webApplicationBuilder, ResourceBuilder resourceBuilder)
    {
        webApplicationBuilder.Logging.AddOpenTelemetry(builder =>
        {
            builder.SetResourceBuilder(resourceBuilder);
        });

        return webApplicationBuilder;
    }

    private static WebApplicationBuilder ConfigureMetrics(this WebApplicationBuilder webApplicationBuilder, ResourceBuilder resourceBuilder)
    {
        webApplicationBuilder.Services.AddOpenTelemetryMetrics(builder =>
        {
            builder
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter(Application.Meter.Name)
                .AddView("http.*.duration", new ExplicitBucketHistogramConfiguration
                {
                    Boundaries = new double[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768 },
                })
                .AddOtlpExporter();
        });

        return webApplicationBuilder;
    }

    private static WebApplicationBuilder ConfigureTracing(this WebApplicationBuilder webApplicationBuilder, ResourceBuilder resourceBuilder)
    {
        bool IgnorePath(PathString path, IEnumerable<string> ignorePatterns) =>
            ignorePatterns.Any(ignorePattern => Regex.IsMatch(path, ignorePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase));

        string[] ignorePatterns = { "^/health$", "^/metrics$" };

        webApplicationBuilder.Services.AddOpenTelemetryTracing(builder =>
        {
            builder
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = httpContext => !IgnorePath(httpContext.Request.Path, ignorePatterns);
                    options.RecordException = true;
                })
                .AddHttpClientInstrumentation()
                .AddNpgsql()
                .AddSource(Application.ActivitySource.Name)
                .AddOtlpExporter();
        });

        return webApplicationBuilder;
    }
}
