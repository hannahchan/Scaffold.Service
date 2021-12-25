namespace Scaffold.Application.Components.Audit;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Scaffold.Application.Common.Messaging;

internal class AuditLogger<TNotification> : AuditLoggerBase, INotificationHandler<TNotification>
    where TNotification : INotification
{
    private readonly ILogger logger;

    public AuditLogger(ILogger<AuditLogger<TNotification>> logger)
    {
        this.logger = logger;
    }

    // Alternatives to logging include saving the event into a database
    // or dispatching it to a dedicated audit service.
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
