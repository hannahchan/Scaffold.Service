namespace Scaffold.Application.Common.Instrumentation;

using System.Diagnostics.Metrics;

internal abstract class EventCounterBase
{
    private protected static readonly Counter<long> EventCounter = MeterProvider.Meter.CreateCounter<long>(
        name: "application.events",
        unit: null,
        description: "measures the number of events that have been published to the in-process event bus");
}
