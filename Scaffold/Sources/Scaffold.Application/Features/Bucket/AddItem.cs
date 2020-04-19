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
            public Command(int bucketId, string? name, string? description)
            {
                this.BucketId = bucketId;
                this.Name = name;
                this.Description = description;
            }

            public int BucketId { get; }

            public string? Name { get; }

            public string? Description { get; }
        }

        public class Response
        {
            public Response(Item item)
            {
                this.Item = item ?? throw new ArgumentNullException(nameof(item));
            }

            public Item Item { get; }
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

                MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));
                Item item = configuration.CreateMapper().Map<Item>(request);

                bucket.AddItem(item);

                await this.repository.UpdateAsync(bucket, cancellationToken);

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
