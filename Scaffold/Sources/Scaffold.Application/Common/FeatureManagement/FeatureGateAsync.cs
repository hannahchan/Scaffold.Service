namespace Scaffold.Application.Common.FeatureManagement;

using System;
using System.Threading.Tasks;

internal class FeatureGateAsync : AbstractFeatureGate
{
    private readonly Func<Task<bool>> controlledBy;

    private readonly Func<Task>? whenOpened;

    private readonly Func<Task>? whenClosed;

    public FeatureGateAsync(string featureGateKey, Func<Task<bool>> controlledBy, Func<Task>? whenOpened, Func<Task>? whenClosed)
        : base(featureGateKey, MetricType.Counter)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public FeatureGateAsync(string featureGateKey, MetricType metricType, Func<Task<bool>> controlledBy, Func<Task>? whenOpened, Func<Task>? whenClosed)
        : base(featureGateKey, metricType)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public async Task Invoke()
    {
        if (await this.controlledBy())
        {
            await this.Invoke(FeatureGateState.Opened, this.whenOpened);
        }
        else
        {
            await this.Invoke(FeatureGateState.Closed, this.whenClosed);
        }
    }

    public FeatureGateAsync WhenOpened(Func<Task>? function)
    {
        return new FeatureGateAsync(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: this.controlledBy,
            whenOpened: function,
            whenClosed: this.whenClosed);
    }

    public FeatureGateAsync WhenOpened(Action? action)
    {
        return new FeatureGateAsync(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: this.controlledBy,
            whenOpened: action == null ? null : () => Task.Run(action),
            whenClosed: this.whenClosed);
    }

    public FeatureGateAsync WhenClosed(Func<Task>? function)
    {
        return new FeatureGateAsync(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: this.controlledBy,
            whenOpened: this.whenOpened,
            whenClosed: function);
    }

    public FeatureGateAsync WhenClosed(Action? action)
    {
        return new FeatureGateAsync(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: this.controlledBy,
            whenOpened: this.whenOpened,
            whenClosed: action == null ? null : () => Task.Run(action));
    }
}
