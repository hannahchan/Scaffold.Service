namespace Scaffold.WebApi.UnitTests;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

    public class HttpRequestHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage response;

        private readonly Exception exception;

        public HttpRequestHandler(HttpResponseMessage response)
        {
            this.response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public HttpRequestHandler(Exception exception)
        {
            this.exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public List<ReceivedRequest> ReceivedRequests { get; } = new List<ReceivedRequest>();

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.ReceivedRequests.Add(new ReceivedRequest(request, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();

            if (this.exception != null)
            {
                throw this.exception;
            }

            this.response.RequestMessage = request;

            return this.response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.Send(request, cancellationToken));
        }

        public record ReceivedRequest(HttpRequestMessage Request, CancellationToken CancellationToken);
    }

    public class ProblemDetailsFactory : Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory
    {
        public override ProblemDetails CreateProblemDetails(
            HttpContext httpContext,
            int? statusCode = null,
            string title = null,
            string type = null,
            string detail = null,
            string instance = null)
        {
            return new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance,
            };
        }

        public override ValidationProblemDetails CreateValidationProblemDetails(
            HttpContext httpContext,
            ModelStateDictionary modelStateDictionary,
            int? statusCode = null,
            string title = null,
            string type = null,
            string detail = null,
            string instance = null)
        {
            return new ValidationProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance,
            };
        }
    }

    public class Sender : ISender
    {
        private object response;

        public List<ReceivedRequest> ReceivedRequests { get; } = new List<ReceivedRequest>();

        public void SetResponse(object response)
        {
            this.response = response;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            this.ReceivedRequests.Add(new ReceivedRequest(request, cancellationToken));
            return Task.FromResult((TResponse)this.response);
        }

        public Task<object> Send(object request, CancellationToken cancellationToken = default)
        {
            this.ReceivedRequests.Add(new ReceivedRequest(request, cancellationToken));
            return Task.FromResult(this.response);
        }

        public record ReceivedRequest(object Request, CancellationToken CancellationToken);
    }
}
