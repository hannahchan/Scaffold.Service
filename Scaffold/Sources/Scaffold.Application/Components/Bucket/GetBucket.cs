namespace Scaffold.Application.Components.Bucket
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Common.Instrumentation;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;

    public static class GetBucket
    {
        public record Query(int Id) : IRequest<Response>;

        public record Response(Bucket Bucket);

        internal class Handler : IRequestHandler<Query, Response>
        {
            private readonly IBucketReadRepository repository;

            public Handler(IBucketReadRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                using Activity? activity = ActivityProvider.StartActivity(nameof(GetBucket));
                return new Response(await this.repository.GetAsync(request.Id, cancellationToken) ?? throw new BucketNotFoundException(request.Id));
            }
        }
    }
}
