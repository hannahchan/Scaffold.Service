namespace Scaffold.Application.Components.Audit;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Scaffold.Application.Common.Messaging;

internal class EventCounter<TNotification> : EventCounterBase, INotificationHandler<TNotification>
    where TNotification : INotification
{
    public Task Handle(TNotification notification, CancellationToken cancellationToken)
    {
        if (notification is IAuditableEvent auditableEvent)
        {
            EventCounter
                .WithLabels(auditableEvent.Type)
                .Inc();

            return Task.CompletedTask;
        }

        EventCounter
            .WithLabels(notification.GetType().Name)
            .Inc();

        return Task.CompletedTask;
    }
}
