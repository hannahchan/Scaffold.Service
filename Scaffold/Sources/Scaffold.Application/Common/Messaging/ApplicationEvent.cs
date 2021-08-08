namespace Scaffold.Application.Common.Messaging
{
    using System;
    using MediatR;

    internal abstract record ApplicationEvent(DateTime Timestamp, string? TraceId, string? Source, string Type, string Description) : INotification;
}
