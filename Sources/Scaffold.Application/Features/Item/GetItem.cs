namespace Scaffold.Application.Features.Item
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Context;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Entities;

    public class GetItem
    {
        public class Query : ApplicationRequest, IRequest<Response>
        {
            public int BucketId { get; set; }

            public int ItemId { get; set; }
        }

        public class Response : ApplicationResponse
        {
            public Item Item { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IBucketRepository repository;

            public Handler(IBucketRepository repository) => this.repository = repository;

            public async Task<Response> Handle(Query query, CancellationToken cancellationToken)
            {
                Bucket bucket = await this.repository.GetAsync(query.BucketId) ??
                    throw new BucketNotFoundException(query.BucketId);

                return new Response { Item = bucket.Items.SingleOrDefault(x => x.Id == query.ItemId) };
            }
        }
    }
}
