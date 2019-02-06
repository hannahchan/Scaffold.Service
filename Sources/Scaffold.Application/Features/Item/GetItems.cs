namespace Scaffold.Application.Features.Item
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Context;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Interfaces;
    using Scaffold.Application.Repositories;
    using Scaffold.Domain.Entities;

    public class GetItems
    {
        public class Query : ApplicationRequest, IRequest<Response>
        {
            public int BucketId { get; set; }
        }

        public class Response : ApplicationResponse
        {
            public IList<Item> Items { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IBucketRepository repository;

            public Handler(IBucketRepository repository) => this.repository = repository;

            public async Task<Response> Handle(Query query, CancellationToken cancellationToken)
            {
                Bucket bucket = await this.repository.GetAsync(query.BucketId) ??
                    throw new BucketNotFoundException(query.BucketId);

                return new Response { Items = bucket.Items.ToList() };
            }
        }
    }
}
