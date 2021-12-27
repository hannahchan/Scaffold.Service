namespace Scaffold.Application.Components.Bucket;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Scaffold.Application.Common.Instrumentation;
using Scaffold.Application.Common.Messaging;
using Scaffold.Domain.Aggregates.Bucket;

public static class RemoveBucket
{
    public record Command(int Id) : IRequest;

    internal class Handler : IRequestHandler<Command>
    {
        private readonly IBucketRepository repository;

        private readonly IPublisher publisher;

        public Handler(IBucketRepository repository, IPublisher publisher)
        {
            this.repository = repository;
            this.publisher = publisher;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            using Activity? activity = ActivityProvider.StartActivity(nameof(RemoveBucket));

            Bucket bucket = await this.repository.GetAsync(request.Id, cancellationToken) ?? throw new BucketNotFoundException(request.Id);

            await this.repository.RemoveAsync(bucket, cancellationToken);
            await this.publisher.Publish(new BucketRemovedEvent(bucket.Id), CancellationToken.None);

            return default;
        }
    }
}
