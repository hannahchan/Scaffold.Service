namespace Scaffold.Application.Components.Bucket;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Scaffold.Application.Common.Instrumentation;
using Scaffold.Application.Common.Interfaces;
using Scaffold.Application.Common.Messaging;
using Scaffold.Domain.Aggregates.Bucket;

public static class GetBucket
{
    public record Query(int Id) : IRequest<Response>;

    public record Response(Bucket Bucket);

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
            using Activity? activity = ActivityProvider.StartActivity(nameof(GetBucket));

            Bucket bucket = await this.repository.GetAsync(request.Id, cancellationToken) ?? throw new BucketNotFoundException(request.Id);
            await this.publisher.Publish(new BucketRetrievedEvent(bucket.Id), CancellationToken.None);

            return new Response(bucket);
        }
    }
}
