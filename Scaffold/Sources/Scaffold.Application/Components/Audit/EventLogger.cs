namespace Scaffold.Application.Components.Audit;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Scaffold.Application.Common.Messaging;

internal class EventLogger<TNotification> : EventLoggerBase, INotificationHandler<TNotification>
    where TNotification : INotification
{
    private readonly ILogger logger;

    public EventLogger(ILogger<EventLogger<TNotification>> logger)
    {
        this.logger = logger;
    }

    public Task Handle(TNotification notification, CancellationToken cancellationToken)
    {
        if (notification is IAuditableEvent auditableEvent)
        {
            LogAuditableEvent(this.logger, auditableEvent.Type, auditableEvent.Description, null);
            return Task.CompletedTask;
        }

        LogNonAuditableEvent(this.logger, notification.GetType().FullName, null);
        return Task.CompletedTask;
    }
}
