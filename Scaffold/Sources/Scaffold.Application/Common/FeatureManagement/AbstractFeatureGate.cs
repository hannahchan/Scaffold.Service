namespace Scaffold.Application.Common.FeatureManagement;

using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using Scaffold.Application.Common.Instrumentation;

internal abstract class AbstractFeatureGate
{
    private static readonly Counter<int> ExecutionCounter = MeterProvider.Meter.CreateCounter<int>(
        name: "feature.gate.executions",
        unit: null,
        description: "measure the number of times a feature gate was executed");

    private static readonly Histogram<double> ExecutionDurationHistogram = MeterProvider.Meter.CreateHistogram<double>(
        name: "feature.gate.duration",
        unit: "ms", // milliseconds
        description: "measures the duration of feature gate executions");

    protected AbstractFeatureGate(string featureGateKey, MetricType metricType)
    {
        this.Key = featureGateKey;
        this.MetricType = metricType;
    }

    protected enum FeatureGateState
    {
        /// <summary>Indicates feature gate is closed.</summary>
        Closed,

        /// <summary>Indicates feature gate is opened.</summary>
        Opened,
    }

    public string Key { get; }

    public MetricType MetricType { get; }

    protected void Invoke(FeatureGateState featureGateState, Action? action)
    {
        TagList tags = CreateTags(this.Key, featureGateState);
        using Activity? activity = StartActivity(this.Key, featureGateState);
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            if (action == null)
            {
                return;
            }

            action();
        }
        catch (Exception exception)
        {
            tags.Add(SemanticConventions.Metrics.FeatureGateExceptionType, exception.GetType().FullName);
            activity?.RecordException(exception);
            throw;
        }
        finally
        {
            stopwatch.Stop();

            switch (this.MetricType)
            {
                case MetricType.Counter:
                    ExecutionCounter.Add(1, tags);
                    break;

                case MetricType.Histogram:
                    ExecutionDurationHistogram.Record(stopwatch.Elapsed.TotalMilliseconds, tags);
                    break;

                default:
                    break;
            }
        }
    }

    protected TResult? Invoke<TResult>(FeatureGateState featureGateState, Func<TResult>? function)
    {
        TagList tags = CreateTags(this.Key, featureGateState);
        using Activity? activity = StartActivity(this.Key, featureGateState);
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            return function == null ? default : function();
        }
        catch (Exception exception)
        {
            tags.Add(SemanticConventions.Metrics.FeatureGateExceptionType, exception.GetType().FullName);
            activity?.RecordException(exception);
            throw;
        }
        finally
        {
            stopwatch.Stop();

            switch (this.MetricType)
            {
                case MetricType.Counter:
                    ExecutionCounter.Add(1, tags);
                    break;

                case MetricType.Histogram:
                    ExecutionDurationHistogram.Record(stopwatch.Elapsed.TotalMilliseconds, tags);
                    break;

                default:
                    break;
            }
        }
    }

    protected async Task Invoke(FeatureGateState featureGateState, Func<Task>? function)
    {
        TagList tags = CreateTags(this.Key, featureGateState);
        using Activity? activity = StartActivity(this.Key, featureGateState);
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            if (function == null)
            {
                return;
            }

            await function();
        }
        catch (Exception exception)
        {
            tags.Add(SemanticConventions.Metrics.FeatureGateExceptionType, exception.GetType().FullName);
            activity?.RecordException(exception);
            throw;
        }
        finally
        {
            stopwatch.Stop();

            switch (this.MetricType)
            {
                case MetricType.Counter:
                    ExecutionCounter.Add(1, tags);
                    break;

                case MetricType.Histogram:
                    ExecutionDurationHistogram.Record(stopwatch.Elapsed.TotalMilliseconds, tags);
                    break;

                default:
                    break;
            }
        }
    }

    protected async Task<TResult?> Invoke<TResult>(FeatureGateState featureGateState, Func<Task<TResult>>? function)
    {
        TagList tags = CreateTags(this.Key, featureGateState);
        using Activity? activity = StartActivity(this.Key, featureGateState);
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            return function == null ? default : await function();
        }
        catch (Exception exception)
        {
            tags.Add(SemanticConventions.Metrics.FeatureGateExceptionType, exception.GetType().FullName);
            activity?.RecordException(exception);
            throw;
        }
        finally
        {
            stopwatch.Stop();

            switch (this.MetricType)
            {
                case MetricType.Counter:
                    ExecutionCounter.Add(1, tags);
                    break;

                case MetricType.Histogram:
                    ExecutionDurationHistogram.Record(stopwatch.Elapsed.TotalMilliseconds, tags);
                    break;

                default:
                    break;
            }
        }
    }

    private static TagList CreateTags(string featureGateKey, FeatureGateState featureGateState)
    {
        return new TagList
        {
            { SemanticConventions.Metrics.FeatureGateKey, featureGateKey },
            { SemanticConventions.Metrics.FeatureGateState, featureGateState },
        };
    }

    private static Activity? StartActivity(string featureGateKey, FeatureGateState featureGateState)
    {
        return ActivityProvider.StartActivity("FeatureGate")
            ?.AddTag(SemanticConventions.Traces.FeatureGateKey, featureGateKey)
            .AddTag(SemanticConventions.Traces.FeatureGateState, featureGateState);
    }

    private static class SemanticConventions
    {
        public static class Metrics
        {
            public const string FeatureGateKey = "key";

            public const string FeatureGateState = "state";

            public const string FeatureGateExceptionType = "exception";
        }

        public static class Traces
        {
            public const string FeatureGateKey = "feature.gate.key";

            public const string FeatureGateState = "feature.gate.state";
        }
    }
}
