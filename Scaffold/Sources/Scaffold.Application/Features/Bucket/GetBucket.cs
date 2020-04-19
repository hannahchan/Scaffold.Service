namespace Scaffold.Application.Features.Bucket
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;

    public static class GetBucket
    {
        public class Query : IRequest<Response>
        {
            public Query(int id)
            {
                this.Id = id;
            }

            public int Id { get; }
        }

        public class Response
        {
            public Response(Bucket bucket)
            {
                this.Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
            }

            public Bucket Bucket { get; }
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
                return new Response(await this.repository.GetAsync(request.Id, cancellationToken) ?? throw new BucketNotFoundException(request.Id));
            }
        }
    }
}
