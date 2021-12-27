namespace Scaffold.Application.Components.Audit;

using System;
using Microsoft.Extensions.Logging;

internal abstract class EventLoggerBase
{
    private protected static readonly Action<ILogger, string, string, Exception?> LogAuditableEvent =
        LoggerMessage.Define<string, string>(LogLevel.Information, default, "{Type} - {Description}");

    private protected static readonly Action<ILogger, string?, Exception?> LogNonAuditableEvent =
        LoggerMessage.Define<string?>(LogLevel.Information, default, "Received Event {Type}");
}
