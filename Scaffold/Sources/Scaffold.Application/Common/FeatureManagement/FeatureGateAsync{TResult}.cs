namespace Scaffold.Application.Common.FeatureManagement;

using System;
using System.Threading.Tasks;

internal class FeatureGateAsync<TResult> : AbstractFeatureGate
{
    private readonly Func<Task<bool>> controlledBy;

    private readonly Func<Task<TResult>>? whenOpened;

    private readonly Func<Task<TResult>>? whenClosed;

    public FeatureGateAsync(string featureGateKey, Func<Task<bool>> controlledBy, Func<Task<TResult>>? whenOpened, Func<Task<TResult>>? whenClosed)
        : base(featureGateKey, MetricType.Counter)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public FeatureGateAsync(string featureGateKey, MetricType metricType, Func<Task<bool>> controlledBy, Func<Task<TResult>>? whenOpened, Func<Task<TResult>>? whenClosed)
        : base(featureGateKey, metricType)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public async Task<TResult?> Invoke()
    {
        return await this.controlledBy()
            ? await this.Invoke(FeatureGateState.Opened, this.whenOpened)
            : await this.Invoke(FeatureGateState.Closed, this.whenClosed);
    }

    public FeatureGateAsync<TResult> WhenOpened(Func<Task<TResult>>? function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: this.controlledBy,
            whenOpened: function,
            whenClosed: this.whenClosed);
    }

    public FeatureGateAsync<TResult> WhenOpened(Func<TResult>? function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: this.controlledBy,
            whenOpened: function == null ? null : () => Task.Run(function),
            whenClosed: this.whenClosed);
    }

    public FeatureGateAsync<TResult> WhenClosed(Func<Task<TResult>>? function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: this.controlledBy,
            whenOpened: this.whenOpened,
            whenClosed: function);
    }

    public FeatureGateAsync<TResult> WhenClosed(Func<TResult>? function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            metricType: this.MetricType,
            controlledBy: this.controlledBy,
            whenOpened: this.whenOpened,
            whenClosed: function == null ? null : () => Task.Run(function));
    }
}
