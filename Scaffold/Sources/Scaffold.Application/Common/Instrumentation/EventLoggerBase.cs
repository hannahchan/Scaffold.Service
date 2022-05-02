namespace Scaffold.Application.Common.Instrumentation;

using Microsoft.Extensions.Logging;

internal abstract partial class EventLoggerBase
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "{Type} - {Description}")]
    private protected static partial void LogAuditableEvent(ILogger logger, string type, string description);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Received Event {Type}")]
    private protected static partial void LogNonAuditableEvent(ILogger logger, string? type);
}
