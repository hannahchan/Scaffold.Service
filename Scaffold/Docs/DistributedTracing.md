# Distributed Tracing #

Application tracing is the practice of profiling and monitoring the execution of operations in an application to pinpoint where failures and performance issues occur. Distributed tracing is an extension to that practice with more focus on tracing operations that cross process boundaries such as those in an application built with microservices architecture.

There are generally three things that you need to do to order to get value out of distributed tracing.

1. Instrument your application
2. Propagate a trace context
3. Collect and report traces

## Instrumenting your application ##

Distributed tracing requires that developers add instrumentation to the code of an application, or to the frameworks used in the application. Scaffold has been configured to use OpenTracing for this purpose. OpenTracing allows developers to add instrumentation to their application code using APIs that do not lock them into one particular tracing product or vendor. OpenTracing also provides out-of-the-box instrumentation for ASP.NET Core apps.

For updated information about OpenTracing and how to instrument your application, please check out;

- https://opentracing.io/
- https://github.com/opentracing/opentracing-csharp
- https://github.com/opentracing-contrib/csharp-netcore

OpenTracing is currently in the process of being merged into a new project called [OpenTelemetry](https://opentelemetry.io/). A future version of Scaffold will probably switch to using OpenTelemetry.

## Propagate a trace context ##

When an operation needs to be traced across process boundaries, a trace context needs to be propagated. For HTTP requests this is usually done via predetermined HTTP headers.

### Built-in Trace Context Propagation in ASP.NET Core ###

Since ASP.NET Core 2.0, ASP.NET Core apps have been propagating a trace context out-of-the-box via the `Request-Id` header using the [Hierarchical](https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/HierarchicalRequestId.md) format. Beginning with ASP.NET Core 3.0, you now have the option to change this to use the [W3C Trace Context](https://github.com/w3c/trace-context) header and format. Future versions of ASP.NET will switch to using the W3C Trace Context specification as default. You can read more about this in the following blog post.

https://devblogs.microsoft.com/aspnet/improvements-in-net-core-3-0-for-troubleshooting-and-monitoring-distributed-apps/

Scaffold has been configured to propagate trace context using the W3C Trace Context specification.

### Other Trace Context Propagation Formats ###

In environments where the trace context propagation format is different from the built-in ones in ASP.NET, developers will need to add further instrumentation to their application code, or to the frameworks used in the application to support that different format. Often this is as simple as configuring the tracing client (tracer) that is used in your application.

## Collect and report traces ##

After instrumenting your application, you will need to configure a tracing client, also called a tracer, to collect and report traces in your application. This tracer is typically registered as the Global Tracer on application startup. For a list of supported tracers, please visit https://opentracing.io/docs/supported-tracers/.

Scaffold has been configured to use [Jaeger](https://www.jaegertracing.io/) as the tracing client for collecting and reporting traces. Documentation for the Jaeger C# client can be found here; https://github.com/jaegertracing/jaeger-client-csharp.
