namespace Scaffold.Application.Common.Instrumentation;

using System.Collections.Generic;
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
            EventCounter.Add(1, new KeyValuePair<string, object?>("name", auditableEvent.Type));
            return Task.CompletedTask;
        }

        EventCounter.Add(1, new KeyValuePair<string, object?>("name", notification.GetType().Name));
        return Task.CompletedTask;
    }
}
