namespace Scaffold.Application.Features.Bucket
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Interfaces;
    using Scaffold.Application.Models;
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

            public Expression<Func<Bucket, bool>> Predicate { get; }

            public int? Limit { get; set; } = null;

            public int? Offset { get; set; } = null;

            public Ordering<Bucket> Ordering { get; set; } = null;
        }

        public class Response
        {
            public IList<Bucket> Buckets { get; set; }
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
                return new Response { Buckets = await this.repository.GetAsync(request.Predicate, request.Limit, request.Offset, request.Ordering) };
            }
        }
    }
}
