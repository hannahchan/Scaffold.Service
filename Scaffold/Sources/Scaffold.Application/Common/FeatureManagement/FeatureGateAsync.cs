namespace Scaffold.Application.Common.FeatureManagement;

using System;
using System.Threading.Tasks;

internal class FeatureGateAsync : AbstractFeatureGate
{
    public FeatureGateAsync(string featureGateKey, Func<Task<bool>> controlledBy, Func<Task>? whenOpened, Func<Task>? whenClosed)
        : base(featureGateKey, MetricType.Counter)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public FeatureGateAsync(string featureGateKey, MetricType metricType, Func<Task<bool>> controlledBy, Func<Task>? whenOpened, Func<Task>? whenClosed)
        : base(featureGateKey, metricType)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public Func<Task<bool>> ControlledBy { get; }

    public Func<Task>? WhenOpened { get; }

    public Func<Task>? WhenClosed { get; }

    public async Task Invoke()
    {
        if (await this.ControlledBy())
        {
            await this.Invoke(FeatureGateState.Opened, this.WhenOpened);
        }
        else
        {
            await this.Invoke(FeatureGateState.Closed, this.WhenClosed);
        }
    }
}
