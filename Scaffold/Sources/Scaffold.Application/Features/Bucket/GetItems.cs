namespace Scaffold.Application.Features.Bucket
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Common.Instrumentation;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;

    public static class GetItems
    {
        public class Query : IRequest<Response>
        {
            public Query(int bucketId)
            {
                this.BucketId = bucketId;
            }

            public int BucketId { get; }
        }

        public class Response
        {
            public Response(IEnumerable<Item> items)
            {
                this.Items = items ?? throw new ArgumentNullException(nameof(items));
            }

            public IEnumerable<Item> Items { get; }
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
                using Activity? activity = ActivityProvider.StartActivity(nameof(GetItems));

                Bucket bucket = await this.repository.GetAsync(request.BucketId, cancellationToken) ??
                    throw new BucketNotFoundException(request.BucketId);

                return new Response(bucket.Items.ToArray());
            }
        }
    }
}
