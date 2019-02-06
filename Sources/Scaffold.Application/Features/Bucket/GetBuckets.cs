namespace Scaffold.Application.Features.Bucket
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Context;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Entities;

    public class GetBuckets
    {
        public class Query : ApplicationRequest, IRequest<Response>
        {
        }

        public class Response : ApplicationResponse
        {
            public IList<Bucket> Buckets { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IBucketRepository repository;

            public Handler(IBucketRepository repository) => this.repository = repository;

            public async Task<Response> Handle(Query query, CancellationToken cancellationToken) =>
                new Response { Buckets = await this.repository.GetAllAsync() };
        }
    }
}
