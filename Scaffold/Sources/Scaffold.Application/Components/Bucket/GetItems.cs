namespace Scaffold.Application.Components.Bucket;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Scaffold.Application.Common.Instrumentation;
using Scaffold.Application.Common.Interfaces;
using Scaffold.Application.Common.Messaging;
using Scaffold.Domain.Aggregates.Bucket;

public static class GetItems
{
    public record Query(int BucketId) : IRequest<Response>;

    public record Response(IEnumerable<Item> Items);

    internal class Handler : IRequestHandler<Query, Response>
    {
        private readonly IBucketReadRepository repository;

        private readonly IPublisher publisher;

        public Handler(IBucketReadRepository repository, IPublisher publisher)
        {
            this.repository = repository;
            this.publisher = publisher;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            using Activity? activity = ActivityProvider.StartActivity(nameof(GetItems));

            Bucket bucket = await this.repository.GetAsync(request.BucketId, cancellationToken) ??
                throw new BucketNotFoundException(request.BucketId);

            await this.publisher.Publish(
                new ItemsRetrievedEvent(bucket.Id, bucket.Items.Select(item => item.Id).ToArray()),
                CancellationToken.None);

            return new Response(bucket.Items.ToArray());
        }
    }
}
