namespace Scaffold.Application.Common.Instrumentation;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Scaffold.Application.Common.Messaging;

internal class EventLogger<TNotification> : EventLoggerBase, INotificationHandler<TNotification>
    where TNotification : INotification
{
    private readonly ILogger logger;

    public EventLogger(ILogger<TNotification> logger)
    {
        this.logger = logger;
    }

    public Task Handle(TNotification notification, CancellationToken cancellationToken)
    {
        if (notification is IAuditableEvent auditableEvent)
        {
            LogAuditableEvent(this.logger, auditableEvent.Type, auditableEvent.Description);
            return Task.CompletedTask;
        }

        LogNonAuditableEvent(this.logger, notification.GetType().FullName);
        return Task.CompletedTask;
    }
}
