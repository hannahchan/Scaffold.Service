namespace Scaffold.Application.Components.Bucket;

using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Scaffold.Application.Common.Instrumentation;
using Scaffold.Application.Common.Messaging;
using Scaffold.Domain.Aggregates.Bucket;

public static class RemoveItem
{
    public record Command(int BucketId, int ItemId) : IRequest;

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
            using Activity? activity = ActivityProvider.StartActivity(nameof(RemoveItem));

            Bucket bucket = await this.repository.GetAsync(request.BucketId, cancellationToken) ?? throw new BucketNotFoundException(request.BucketId);
            Item item = bucket.Items.SingleOrDefault(x => x.Id == request.ItemId) ?? throw new ItemNotFoundException(request.ItemId);

            bucket.RemoveItem(item);

            await this.repository.UpdateAsync(bucket, cancellationToken);
            await this.publisher.Publish(new ItemRemovedEvent(bucket.Id, item.Id), CancellationToken.None);

            return default;
        }
    }
}
