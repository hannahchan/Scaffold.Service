namespace Scaffold.WebApi.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;

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

        public class Sender : ISender
        {
            private object response;

            public void SetResponse(object response)
            {
                this.response = response;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult((TResponse)this.response);
            }

            public Task<object> Send(object request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(this.response);
            }
        }
    }
}
