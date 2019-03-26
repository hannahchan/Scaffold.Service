namespace Scaffold.Application.Features.Bucket
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Entities;

    public class GetBuckets
    {
        public class Query : IRequest<Response>
        {
            public Query() =>
                this.Predicate = bucket => true;

            public Query(Expression<Func<Bucket, bool>> predicate) =>
                this.Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

            public Expression<Func<Bucket, bool>> Predicate { get; }
        }

        public class Response
        {
            public IList<Bucket> Buckets { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IBucketRepository repository;

            public Handler(IBucketRepository repository) => this.repository = repository;

            public async Task<Response> Handle(Query query, CancellationToken cancellationToken) =>
                new Response { Buckets = await this.repository.GetAsync(query.Predicate) };
        }
    }
}
