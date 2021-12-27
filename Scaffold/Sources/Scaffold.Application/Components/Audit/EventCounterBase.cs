namespace Scaffold.Application.Components.Audit;

using Prometheus;

internal abstract class EventCounterBase
{
    private protected static readonly Counter EventCounter = Metrics.CreateCounter(
        "application_events_total",
        "Total number of events that have been published to the in-process event bus",
        new CounterConfiguration { LabelNames = new[] { "name" } });
}
