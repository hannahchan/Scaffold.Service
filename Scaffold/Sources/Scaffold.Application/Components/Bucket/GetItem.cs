namespace Scaffold.Application.Components.Bucket
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Common.Instrumentation;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;

    public static class GetItem
    {
        public record Query(int BucketId, int ItemId) : IRequest<Response>;

        public record Response(Item Item);

        internal class Handler : IRequestHandler<Query, Response>
        {
            private readonly IBucketReadRepository repository;

            public Handler(IBucketReadRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                using Activity? activity = ActivityProvider.StartActivity(nameof(GetItem));

                Bucket bucket = await this.repository.GetAsync(request.BucketId, cancellationToken) ??
                    throw new BucketNotFoundException(request.BucketId);

                return new Response(bucket.Items.SingleOrDefault(x => x.Id == request.ItemId) ??
                    throw new ItemNotFoundException(request.ItemId));
            }
        }
    }
}
