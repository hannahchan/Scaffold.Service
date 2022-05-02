namespace Scaffold.Application.Common.Instrumentation;

using System.Diagnostics.Metrics;
using Scaffold.Application.Common.Constants;

internal static class MeterProvider
{
    public static readonly Meter Meter = new Meter(Application.Meter.Name, Application.Meter.Version);
}
