namespace Scaffold.Application.Features.Bucket
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;

    public static class UpdateItem
    {
        public class Command : IRequest<Response>
        {
            public Command(int bucketId, int itemId, string? name, string? description)
            {
                this.BucketId = bucketId;
                this.ItemId = itemId;
                this.Name = name;
                this.Description = description;
            }

            public int BucketId { get; }

            public int ItemId { get; }

            public string? Name { get; }

            public string? Description { get; }
        }

        public class Response
        {
            public Response(Item item, bool created = false)
            {
                this.Item = item ?? throw new ArgumentNullException(nameof(item));
                this.Created = created;
            }

            public Item Item { get; }

            public bool Created { get; }

            public bool Updated { get => !this.Created; }
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
                Bucket bucket = await this.repository.GetAsync(request.BucketId, cancellationToken) ??
                    throw new BucketNotFoundException(request.BucketId);

                Item item = bucket.Items.SingleOrDefault(x => x.Id == request.ItemId);

                MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));

                if (item is null)
                {
                    item = configuration.CreateMapper().Map<Item>(request);
                    bucket.AddItem(item);
                    await this.repository.UpdateAsync(bucket, cancellationToken);

                    return new Response(item, true);
                }

                item = configuration.CreateMapper().Map(request, item);
                await this.repository.UpdateAsync(bucket, cancellationToken);

                return new Response(item);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                this.CreateMap<Command, Item>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ItemId));
            }
        }
    }
}
