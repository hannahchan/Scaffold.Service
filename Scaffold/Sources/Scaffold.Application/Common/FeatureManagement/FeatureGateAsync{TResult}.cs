namespace Scaffold.Application.Common.FeatureManagement;

using System;
using System.Threading.Tasks;

internal class FeatureGateAsync<TResult> : AbstractFeatureGate
{
    public FeatureGateAsync(string featureGateKey, Func<Task<bool>> controlledBy, Func<Task<TResult>>? whenOpened, Func<Task<TResult>>? whenClosed)
        : base(featureGateKey, MetricType.Counter)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public FeatureGateAsync(string featureGateKey, MetricType metricType, Func<Task<bool>> controlledBy, Func<Task<TResult>>? whenOpened, Func<Task<TResult>>? whenClosed)
        : base(featureGateKey, metricType)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public Func<Task<bool>> ControlledBy { get; }

    public Func<Task<TResult>>? WhenOpened { get; }

    public Func<Task<TResult>>? WhenClosed { get; }

    public async Task<TResult?> Invoke()
    {
        return await this.ControlledBy()
            ? await this.Invoke(FeatureGateState.Opened, this.WhenOpened)
            : await this.Invoke(FeatureGateState.Closed, this.WhenClosed);
    }
}
