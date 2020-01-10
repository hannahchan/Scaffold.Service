namespace Scaffold.Application.Features.Bucket
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;

    public static class AddItem
    {
        public class Command : IRequest<Response>
        {
            public int BucketId { get; set; }

            public string? Name { get; set; }

            public string? Description { get; set; }
        }

        public class Response
        {
            public Response(Item item)
            {
                this.Item = item ?? throw new ArgumentNullException(nameof(item));
            }

            public Item Item { get; private set; }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IBucketRepository repository;

            public Handler(IBucketRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                Bucket bucket = await this.repository.GetAsync(request.BucketId) ??
                    throw new BucketNotFoundException(request.BucketId);

                MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));
                Item item = configuration.CreateMapper().Map<Item>(request);

                bucket.AddItem(item);

                await this.repository.UpdateAsync(bucket);

                return new Response(item);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                this.CreateMap<Command, Item>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore());
            }
        }
    }
}
