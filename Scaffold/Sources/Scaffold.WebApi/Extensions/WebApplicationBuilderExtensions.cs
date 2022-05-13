namespace Scaffold.WebApi.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scaffold.Application.Common.Constants;

internal static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        IConfiguration configuration = builder.Configuration;

        string serviceName = configuration.GetValue<string>("OpenTelemetry:ServiceName");
        string? serviceNamespace = configuration.GetValue<string?>("OpenTelemetry:ServiceNamespace");
        string? serviceVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        string? serviceInstanceId = Environment.MachineName;

        ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceNamespace, serviceVersion, autoGenerateServiceInstanceId: true, serviceInstanceId);

        void ConfigureOtlpExporter(OtlpExporterOptions options)
        {
            options.Endpoint = new Uri(configuration.GetValue("OpenTelemetry:Otlp:Endpoint", "http://localhost:4317"));
            options.ExportProcessorType = configuration.GetValue("OpenTelemetry:Otlp:ExportProcessorType", ExportProcessorType.Batch);
            options.Protocol = configuration.GetValue("OpenTelemetry:Otlp:Protocol", OtlpExportProtocol.Grpc);
            options.TimeoutMilliseconds = configuration.GetValue("OpenTelemetry:Otlp:TimeoutMilliseconds", 10000);
        }

        return builder
            .ConfigureLogging(resourceBuilder)
            .ConfigureMetrics(resourceBuilder, ConfigureOtlpExporter)
            .ConfigureTracing(resourceBuilder, ConfigureOtlpExporter);
    }

    private static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder webApplicationBuilder, ResourceBuilder resourceBuilder)
    {
        webApplicationBuilder.Logging.AddOpenTelemetry(builder =>
        {
            builder.SetResourceBuilder(resourceBuilder);
        });

        return webApplicationBuilder;
    }

    private static WebApplicationBuilder ConfigureMetrics(this WebApplicationBuilder webApplicationBuilder, ResourceBuilder resourceBuilder, Action<OtlpExporterOptions> options)
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
                .AddOtlpExporter(options);
        });

        return webApplicationBuilder;
    }

    private static WebApplicationBuilder ConfigureTracing(this WebApplicationBuilder webApplicationBuilder, ResourceBuilder resourceBuilder, Action<OtlpExporterOptions> options)
    {
        bool IgnorePath(PathString path, IEnumerable<string> ignorePatterns) =>
            ignorePatterns.Any(ignorePattern => Regex.IsMatch(path, ignorePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase));

        string[] ignorePatterns = { "^/health$", "^/metrics$" };

        webApplicationBuilder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = httpContext => !IgnorePath(httpContext.Request.Path, ignorePatterns);
                    options.RecordException = true;
                })
                .AddHttpClientInstrumentation()
                .AddNpgsql()
                .AddSource(Application.ActivitySource.Name)
                .AddOtlpExporter(options);
        });

        return webApplicationBuilder;
    }
}
