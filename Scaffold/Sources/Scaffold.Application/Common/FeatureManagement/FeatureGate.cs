namespace Scaffold.Application.Common.FeatureManagement;

using System;

internal class FeatureGate : AbstractFeatureGate
{
    public FeatureGate(string featureGateKey, Func<bool> controlledBy, Action? whenOpened, Action? whenClosed)
        : base(featureGateKey, MetricType.Counter)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public FeatureGate(string featureGateKey, MetricType metricType, Func<bool> controlledBy, Action? whenOpened, Action? whenClosed)
        : base(featureGateKey, metricType)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public Func<bool> ControlledBy { get; }

    public Action? WhenOpened { get; }

    public Action? WhenClosed { get; }

    public void Invoke()
    {
        if (this.ControlledBy())
        {
            this.Invoke(FeatureGateState.Opened, this.WhenOpened);
        }
        else
        {
            this.Invoke(FeatureGateState.Closed, this.WhenClosed);
        }
    }
}
