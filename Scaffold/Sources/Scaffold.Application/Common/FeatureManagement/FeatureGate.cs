namespace Scaffold.Application.Common.FeatureManagement;

using System;
using System.Threading.Tasks;

internal class FeatureGate : AbstractFeatureGate
{
    private readonly Func<bool> controlledBy;

    private readonly Action? whenOpened;

    private readonly Action? whenClosed;

    public FeatureGate(string featureGateKey, Func<bool> controlledBy, Action? whenOpened, Action? whenClosed)
        : base(featureGateKey, MetricType.Counter)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public FeatureGate(string featureGateKey, MetricType metricType, Func<bool> controlledBy, Action? whenOpened, Action? whenClosed)
        : base(featureGateKey, metricType)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public static FeatureGateBuilder WithKey(string featureGateKey)
    {
        return new FeatureGateBuilder(featureGateKey);
    }

    public void Invoke()
    {
        if (this.controlledBy())
        {
            this.Invoke(FeatureGateState.Opened, this.whenOpened);
        }
        else
        {
            this.Invoke(FeatureGateState.Closed, this.whenClosed);
        }
    }

    public FeatureGate WhenOpened(Action? action)
    {
        return new FeatureGate(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: this.controlledBy,
            whenOpened: action,
            whenClosed: this.whenClosed);
    }

    public FeatureGateAsync WhenOpened(Func<Task>? function)
    {
        return new FeatureGateAsync(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: () => Task.Run(this.controlledBy),
            whenOpened: function,
            whenClosed: this.whenClosed == null ? null : () => Task.Run(this.whenClosed));
    }

    public FeatureGate WhenClosed(Action? action)
    {
        return new FeatureGate(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: this.controlledBy,
            whenOpened: this.whenOpened,
            whenClosed: action);
    }

    public FeatureGateAsync WhenClosed(Func<Task>? function)
    {
        return new FeatureGateAsync(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: () => Task.Run(this.controlledBy),
            whenOpened: this.whenOpened == null ? null : () => Task.Run(this.whenOpened),
            whenClosed: function);
    }
}
