namespace Scaffold.Application.Features.Bucket
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Context;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Entities;

    public class GetBucket
    {
        public class Query : ApplicationRequest, IRequest<Response>
        {
            public int Id { get; set; }
        }

        public class Response : ApplicationResponse
        {
            public Bucket Bucket { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IBucketRepository repository;

            public Handler(IBucketRepository repository) => this.repository = repository;

            public async Task<Response> Handle(Query query, CancellationToken cancellationToken) =>
                new Response { Bucket = await this.repository.GetAsync(query.Id) };
        }
    }
}
