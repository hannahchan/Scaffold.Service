namespace Scaffold.Application.UnitTests;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

// Contains shared and reusable mocks
public static class Mock
{
    public class Logger<T> : ILogger<T>
    {
        public List<LogEntry> LogEntries { get; } = new List<LogEntry>();

        public List<object> Scopes { get; } = new List<object>();

        public IDisposable BeginScope<TState>(TState state)
        {
            this.Scopes.Add(state);
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            this.LogEntries.Add(new LogEntry(typeof(T).FullName, logLevel, eventId, formatter(state, exception), exception));
        }

        public sealed class NullScope : IDisposable
        {
            private NullScope()
            {
            }

            public static NullScope Instance { get; } = new NullScope();

            public void Dispose()
            {
                // Do nothing
            }
        }

        public record LogEntry(string CategoryName, LogLevel LogLevel, EventId EventId, string Message, Exception Exception);
    }

    public class Publisher : IPublisher
    {
        public List<Event> PublishedEvents { get; } = new List<Event>();

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            this.PublishedEvents.Add(new Event(notification, cancellationToken));
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            this.PublishedEvents.Add(new Event(notification, cancellationToken));
            return Task.CompletedTask;
        }

        public record Event(object Notification, CancellationToken CancellationToken);
    }
}
