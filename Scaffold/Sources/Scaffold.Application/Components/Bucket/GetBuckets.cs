namespace Scaffold.Application.Components.Bucket
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Common.Instrumentation;
    using Scaffold.Application.Common.Models;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;

    public static class GetBuckets
    {
        public record Query(Expression<Func<Bucket, bool>> Predicate, int? Limit = null, int? Offset = null, SortOrder<Bucket>? SortOrder = null) : IRequest<Response>;

        public record Response(IEnumerable<Bucket> Buckets);

        internal class Handler : IRequestHandler<Query, Response>
        {
            private readonly IBucketReadRepository repository;

            public Handler(IBucketReadRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                using Activity? activity = ActivityProvider.StartActivity(nameof(GetBuckets));
                return new Response(await this.repository.GetAsync(request.Predicate, request.Limit, request.Offset, request.SortOrder, cancellationToken));
            }
        }
    }
}
