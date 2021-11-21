namespace Scaffold.Application.Common.Messaging;

using System;

internal interface IAuditableEvent
{
    DateTime Timestamp { get; }

    string? TraceId { get; }

    string? Source { get; }

    string Type { get; }

    string Description { get; }
}
