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
        public class Query : IRequest<Response>
        {
            public Query()
            {
                this.Predicate = bucket => true;
            }

            public Query(Expression<Func<Bucket, bool>> predicate)
            {
                this.Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            }

            public Query(Expression<Func<Bucket, bool>> predicate, int? limit, int? offset, SortOrder<Bucket>? sortOrder)
                : this(predicate)
            {
                this.Limit = limit;
                this.Offset = offset;
                this.SortOrder = sortOrder;
            }

            public Expression<Func<Bucket, bool>> Predicate { get; }

            public int? Limit { get; } = null;

            public int? Offset { get; } = null;

            public SortOrder<Bucket>? SortOrder { get; } = null;
        }

        public class Response
        {
            public Response(IEnumerable<Bucket> buckets)
            {
                this.Buckets = buckets ?? throw new ArgumentNullException(nameof(buckets));
            }

            public IEnumerable<Bucket> Buckets { get; }
        }

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