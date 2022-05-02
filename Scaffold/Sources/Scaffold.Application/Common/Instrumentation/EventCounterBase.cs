namespace Scaffold.Application.Common.Instrumentation;

using System;
using System.Diagnostics.Metrics;
using Prometheus;

internal abstract class EventCounterBase
{
    private protected static readonly Counter<long> EventCounter = MeterProvider.Meter.CreateCounter<long>(
        name: "application.events.count",
        unit: "events",
        description: "measures the number of events that have been published to the in-process event bus");

    [Obsolete("Replaced by native .NET Metrics Counter")]
    private protected static readonly Counter EventCounterLegacy = Metrics.CreateCounter(
        "application_events_total",
        "Total number of events that have been published to the in-process event bus",
        new CounterConfiguration { LabelNames = new[] { "name" } });
}
