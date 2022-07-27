namespace Scaffold.Application.Common.FeatureManagement;

using System;
using System.Threading.Tasks;

internal class FeatureGateBuilder
{
    public FeatureGateBuilder(string featureGateKey)
    {
        this.Key = featureGateKey;
    }

    public string Key { get; }

    public MetricType MetricType { get; private set; } = MetricType.Counter;

    public FeatureGateBuilder WithHistogram()
    {
        this.MetricType = MetricType.Histogram;
        return this;
    }

    public PartialFeatureGate ControlledBy(Func<bool> predicate)
    {
        return new PartialFeatureGate(this.Key, this.MetricType, predicate);
    }

    public PartialFeatureGateAsync ControlledBy(Func<Task<bool>> predicate)
    {
        return new PartialFeatureGateAsync(this.Key, this.MetricType, predicate);
    }

    internal class PartialFeatureGate
    {
        public PartialFeatureGate(string key, MetricType metricType, Func<bool> controlledBy)
        {
            this.Key = key;
            this.MetricType = metricType;
            this.ControlledBy = controlledBy;
        }

        public string Key { get; }

        public MetricType MetricType { get; }

        public Func<bool> ControlledBy { get; }

        public FeatureGate WhenOpened(Action? action)
        {
            return new FeatureGate(this.Key, this.MetricType, this.ControlledBy, action, null);
        }

        public FeatureGate<TResult> WhenOpened<TResult>(Func<TResult> function)
        {
            return new FeatureGate<TResult>(this.Key, this.MetricType, this.ControlledBy, function, null);
        }

        public FeatureGateAsync WhenOpened(Func<Task> function)
        {
            return new FeatureGateAsync(this.Key, this.MetricType, () => Task.Run(this.ControlledBy), function, null);
        }

        public FeatureGateAsync<TResult> WhenOpened<TResult>(Func<Task<TResult>> function)
        {
            return new FeatureGateAsync<TResult>(this.Key, this.MetricType, () => Task.Run(this.ControlledBy), function, null);
        }
    }

    internal class PartialFeatureGateAsync
    {
        public PartialFeatureGateAsync(string key, MetricType metricType, Func<Task<bool>> controlledBy)
        {
            this.Key = key;
            this.MetricType = metricType;
            this.ControlledBy = controlledBy;
        }

        public string Key { get; }

        public MetricType MetricType { get; }

        public Func<Task<bool>> ControlledBy { get; }

        public FeatureGateAsync WhenOpened(Func<Task> function)
        {
            return new FeatureGateAsync(this.Key, this.MetricType, this.ControlledBy, function, null);
        }

        public FeatureGateAsync<TResult> WhenOpened<TResult>(Func<Task<TResult>> function)
        {
            return new FeatureGateAsync<TResult>(this.Key, this.MetricType, this.ControlledBy, function, null);
        }

        public FeatureGateAsync WhenOpened(Action action)
        {
            return this.WhenOpened(() => Task.Run(action));
        }

        public FeatureGateAsync<TResult> WhenOpened<TResult>(Func<TResult> function)
        {
            return this.WhenOpened(() => Task.Run(function));
        }
    }
}
