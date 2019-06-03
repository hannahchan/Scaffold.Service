namespace Scaffold.Application.Features.Bucket
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Entities;

    public class RemoveBucket
    {
        public class Command : IRequest
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IBucketRepository repository;

            public Handler(IBucketRepository repository) => this.repository = repository;

            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                Bucket bucket = await this.repository.GetAsync(command.Id);

                if (bucket != null)
                {
                    await this.repository.RemoveAsync(bucket);
                }

                return default;
            }
        }
    }
}
