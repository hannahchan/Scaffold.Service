namespace Scaffold.Application.Common.FeatureManagement;

using System;

internal class FeatureGate<TResult> : AbstractFeatureGate
{
    public FeatureGate(string featureGateKey, Func<bool> controlledBy, Func<TResult>? whenOpened, Func<TResult>? whenClosed)
        : base(featureGateKey, MetricType.Counter)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public FeatureGate(string featureGateKey, MetricType metricType, Func<bool> controlledBy, Func<TResult>? whenOpened, Func<TResult>? whenClosed)
        : base(featureGateKey, metricType)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public Func<bool> ControlledBy { get; }

    public Func<TResult>? WhenOpened { get; }

    public Func<TResult>? WhenClosed { get; }

    public TResult? Invoke()
    {
        return this.ControlledBy()
            ? this.Invoke(FeatureGateState.Opened, this.WhenOpened)
            : this.Invoke(FeatureGateState.Closed, this.WhenClosed);
    }
}
