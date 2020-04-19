namespace Scaffold.Application.Features.Bucket
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;

    public static class GetItem
    {
        public class Query : IRequest<Response>
        {
            public Query(int bucketId, int itemId)
            {
                this.BucketId = bucketId;
                this.ItemId = itemId;
            }

            public int BucketId { get; }

            public int ItemId { get; }
        }

        public class Response
        {
            public Response(Item item)
            {
                this.Item = item ?? throw new ArgumentNullException(nameof(item));
            }

            public Item Item { get; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IBucketReadRepository repository;

            public Handler(IBucketReadRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                Bucket bucket = await this.repository.GetAsync(request.BucketId, cancellationToken) ??
                    throw new BucketNotFoundException(request.BucketId);

                return new Response(bucket.Items.SingleOrDefault(x => x.Id == request.ItemId) ??
                    throw new ItemNotFoundException(request.ItemId));
            }
        }
    }
}
