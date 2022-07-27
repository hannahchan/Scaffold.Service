namespace Scaffold.Application.Common.FeatureManagement;

using System;
using System.Threading.Tasks;

internal class FeatureGate<TResult> : AbstractFeatureGate
{
    private readonly Func<bool> controlledBy;

    private readonly Func<TResult>? whenOpened;

    private readonly Func<TResult>? whenClosed;

    public FeatureGate(string featureGateKey, Func<bool> controlledBy, Func<TResult>? whenOpened, Func<TResult>? whenClosed)
        : base(featureGateKey, MetricType.Counter)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public FeatureGate(string featureGateKey, MetricType metricType, Func<bool> controlledBy, Func<TResult>? whenOpened, Func<TResult>? whenClosed)
        : base(featureGateKey, metricType)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public TResult? Invoke()
    {
        return this.controlledBy()
            ? this.Invoke(FeatureGateState.Opened, this.whenOpened)
            : this.Invoke(FeatureGateState.Closed, this.whenClosed);
    }

    public FeatureGate<TResult> WhenOpened(Func<TResult>? function)
    {
        return new FeatureGate<TResult>(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: this.controlledBy,
            whenOpened: function,
            whenClosed: this.whenClosed);
    }

    public FeatureGateAsync<TResult> WhenOpened(Func<Task<TResult>>? function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: () => Task.Run(this.controlledBy),
            whenOpened: function,
            whenClosed: this.whenClosed == null ? null : () => Task.Run(this.whenClosed));
    }

    public FeatureGate<TResult> WhenClosed(Func<TResult>? function)
    {
        return new FeatureGate<TResult>(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: this.controlledBy,
            whenOpened: this.whenOpened,
            whenClosed: function);
    }

    public FeatureGateAsync<TResult> WhenClosed(Func<Task<TResult>>? function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: () => Task.Run(this.controlledBy),
            whenOpened: this.whenOpened == null ? null : () => Task.Run(this.whenOpened),
            whenClosed: function);
    }
}
