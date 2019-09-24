namespace Scaffold.Application.Features.Item
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
            public int BucketId { get; set; }

            public int ItemId { get; set; }
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
                Bucket bucket = await this.repository.GetAsync(request.BucketId);
                Item item = bucket?.Items.SingleOrDefault(x => x.Id == request.ItemId);

                if (item != null)
                {
                    bucket.RemoveItem(item);
                    await this.repository.UpdateAsync(bucket);
                }

                return default;
            }
        }
    }
}
