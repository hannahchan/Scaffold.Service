namespace Scaffold.Application.Features.Bucket
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Entities;

    public class GetBucket
    {
        public class Query : IRequest<Response>
        {
            public int Id { get; set; }
        }

        public class Response
        {
            public Bucket Bucket { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IBucketReadRepository repository;

            public Handler(IBucketReadRepository repository) => this.repository = repository;

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken) =>
                new Response { Bucket = await this.repository.GetAsync(request.Id) };
        }
    }
}
