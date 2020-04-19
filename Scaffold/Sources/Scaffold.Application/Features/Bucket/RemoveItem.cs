namespace Scaffold.Application.Features.Bucket
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;

    public static class RemoveItem
    {
        public class Command : IRequest
        {
            public Command(int bucketId, int itemId)
            {
                this.BucketId = bucketId;
                this.ItemId = itemId;
            }

            public int BucketId { get; }

            public int ItemId { get; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IBucketRepository repository;

            public Handler(IBucketRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                Bucket bucket = await this.repository.GetAsync(request.BucketId, cancellationToken) ?? throw new BucketNotFoundException(request.BucketId);
                Item item = bucket.Items.SingleOrDefault(x => x.Id == request.ItemId) ?? throw new ItemNotFoundException(request.ItemId);

                bucket.RemoveItem(item);
                await this.repository.UpdateAsync(bucket, cancellationToken);

                return default;
            }
        }
    }
}
