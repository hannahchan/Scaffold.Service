namespace Scaffold.Application.Components.Bucket
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Scaffold.Application.Common.Instrumentation;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;

    public static class UpdateItem
    {
        public record Command(int BucketId, int ItemId, string? Name, string? Description) : IRequest<Response>;

        public record Response(Item Item, bool Created);

        internal class Handler : IRequestHandler<Command, Response>
        {
            private readonly IBucketRepository repository;

            public Handler(IBucketRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                using Activity? activity = ActivityProvider.StartActivity(nameof(UpdateItem));

                Bucket bucket = await this.repository.GetAsync(request.BucketId, cancellationToken) ??
                    throw new BucketNotFoundException(request.BucketId);

                Item? item = bucket.Items.SingleOrDefault(x => x.Id == request.ItemId);

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

                return new Response(item, false);
            }
        }

        internal class MappingProfile : Profile
        {
            public MappingProfile()
            {
                this.CreateMap<Command, Item>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ItemId));
            }
        }
    }
}
