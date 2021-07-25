namespace Scaffold.Application.Components.Bucket
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Common.Instrumentation;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;

    public static class RemoveBucket
    {
        public record Command(int Id) : IRequest;

        internal class Handler : IRequestHandler<Command>
        {
            private readonly IBucketRepository repository;

            public Handler(IBucketRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                using Activity? activity = ActivityProvider.StartActivity(nameof(RemoveBucket));

                Bucket bucket = await this.repository.GetAsync(request.Id, cancellationToken) ?? throw new BucketNotFoundException(request.Id);
                await this.repository.RemoveAsync(bucket, cancellationToken);

                return default;
            }
        }
    }
}
