namespace Scaffold.HttpClients.UnitTests;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// Contains shared and reusable mocks
public static class Mock
{
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
}
