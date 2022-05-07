# Tracing

Scaffold uses [OpenTelemetry](https://opentelemetry.io) for distributed tracing and can be configured to send traces to an [OpenTelemetry collector](https://opentelemetry.io/docs/collector/) or directly to your backend of choice. Using an [OpenTelemetry collector](https://opentelemetry.io/docs/collector/) is recommended as it allows you to keep Scaffold abstracted away from your tracing backend.

For local development, Scaffold has been configured to send traces to an OpenTelemetry collector which then forwards them to a local Jaeger instance. You can view the traces at <http://localhost:16686/search>. It is also possible to view the traces in the local Grafana instance at <http://localhost:3000>.

For more details about using OpenTelemetry in .NET, please check out the [OpenTelemetry .NET](https://github.com/open-telemetry/opentelemetry-dotnet) project.

## Instrumenting Code

To instrument your code with traces, you can use the [OpenTelemetry .NET API](https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/src/OpenTelemetry.Api) or the native .NET Activity APIs. Using the native .NET Activity APIs is preferred.

An internal static [_ActivityProvider_](../Sources/Scaffold.Application/Common/Instrumentation/ActivityProvider.cs) which wraps around some of the .NET Activity APIs has been provided in Scaffold to help you instrument the application layer. The following is an example of how to use it.

```C#
using (Activity? activity = ActivityProvider.StartActivity("ActivityName"))
{
    // Perform activity here!
}
```
