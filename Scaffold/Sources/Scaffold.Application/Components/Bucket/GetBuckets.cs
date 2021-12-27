namespace Scaffold.Application.Components.Bucket;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Scaffold.Application.Common.Instrumentation;
using Scaffold.Application.Common.Interfaces;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Common.Models;
using Scaffold.Domain.Aggregates.Bucket;

public static class GetBuckets
{
    public record Query(Expression<Func<Bucket, bool>> Predicate, int? Limit = null, int? Offset = null, SortOrder<Bucket>? SortOrder = null) : IRequest<Response>;

    public record Response(IEnumerable<Bucket> Buckets);

    internal class Handler : IRequestHandler<Query, Response>
    {
        private readonly IBucketReadRepository repository;

        private readonly IPublisher publisher;

        public Handler(IBucketReadRepository repository, IPublisher publisher)
        {
            this.repository = repository;
            this.publisher = publisher;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            using Activity? activity = ActivityProvider.StartActivity(nameof(GetBuckets));

            List<Bucket> buckets = await this.repository.GetAsync(request.Predicate, request.Limit, request.Offset, request.SortOrder, cancellationToken);
            await this.publisher.Publish(new BucketsRetrievedEvent(buckets.Select(bucket => bucket.Id).ToArray()), CancellationToken.None);

            return new Response(buckets);
        }
    }
}
