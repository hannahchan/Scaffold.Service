namespace Scaffold.Application.Features.Item
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Entities;

    public static class GetItems
    {
        public class Query : IRequest<Response>
        {
            public int BucketId { get; set; }
        }

        public class Response
        {
            public IList<Item> Items { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IBucketReadRepository repository;

            public Handler(IBucketReadRepository repository) => this.repository = repository;

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                Bucket bucket = await this.repository.GetAsync(request.BucketId) ??
                    throw new BucketNotFoundException(request.BucketId);

                return new Response { Items = bucket.Items.ToList() };
            }
        }
    }
}
