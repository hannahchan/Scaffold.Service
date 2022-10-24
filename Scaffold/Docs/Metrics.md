# Metrics

Scaffold uses [OpenTelemetry](https://opentelemetry.io) for application metrics and can be configured to send metrics to an [OpenTelemetry collector](https://opentelemetry.io/docs/collector/) or directly to your backend of choice. Using an [OpenTelemetry collector](https://opentelemetry.io/docs/collector/) is recommended as it allows you to keep Scaffold abstracted away from your metrics backend.

For local development, Scaffold has been configured to send metrics to an OpenTelemetry collector which is then scraped by a local Prometheus instance. You can view the metrics at <http://localhost:9090/graph>. It is also possible to view the metrics in the local Grafana instance at <http://localhost:3000>.

For more details about using OpenTelemetry in .NET, please check out the [OpenTelemetry .NET](https://github.com/open-telemetry/opentelemetry-dotnet) project.

## Instrumenting Code

To instrument your code with metrics, you can use the native [.NET Metrics API](https://learn.microsoft.com/dotnet/core/diagnostics/metrics). For for more information on how to instrument code for OpenTelemetry, please refer to documentation in the [OpenTelemetry .NET API](https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/src/OpenTelemetry.Api).

### Meter Provider

An internal static [_MeterProvider_](../Sources/Scaffold.Application/Common/Instrumentation/MeterProvider.cs) has been provided in Scaffold to help you instrument the application layer. This `MeterProvider`, which is distinct from the OpenTelemetry MeterProvider, holds the static instance of a Meter for the application layer.

### Event Counter

Included in the application layer of Scaffold is an [_EventCounter_](../Sources/Scaffold.Application/Components/Audit/EventCounter.cs) that counts all the messages that pass through the [in-process event bus](./Architecture.md). Rather than littering the codebase with counters, developers can simply publish an event and it will be counted with all the other events of the same type and emitted as a metric.
