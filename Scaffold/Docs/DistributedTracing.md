# Distributed Tracing

Application tracing is the practice of profiling and monitoring the execution of operations in an application to pinpoint where failures and performance issues occur. Distributed tracing is an extension to that practice with more focus on tracing operations that cross process boundaries such as those in an application built with microservices architecture.

Scaffold uses the [OpenTelemetry](https://opentelemetry.io) observability framework for distributed tracing. For more details about this implementation, please check out the [OpenTelemetry .NET](https://github.com/open-telemetry/opentelemetry-dotnet) project.

## Instrumenting Code

An internal static class [`ActivityProvider`](../Sources/Scaffold.Application/Common/Instrumentation/ActivityProvider.cs) has also been provided in Scaffold to help you instrument the application layer. The following is an example of how to use it.

```C#
using (Activity? activity = ActivityProvider.StartActivity("ActivityName"))
{
    // Perform activity here!
}
```

For for more information on how to instrument code for OpenTelemetry, please refer to documentation in the [OpenTelemetry .NET API](https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/src/OpenTelemetry.Api).
