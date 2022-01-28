namespace Scaffold.Application.Components.Bucket;

using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Scaffold.Application.Common.Instrumentation;
using Scaffold.Application.Common.Messaging;
using Scaffold.Domain.Aggregates.Bucket;

public static class UpdateItem
{
    public record Command(int BucketId, int ItemId, string? Name, string? Description) : IRequest<Response>;

    public record Response(Item Item, bool Created);

    internal class Handler : IRequestHandler<Command, Response>
    {
        private static readonly IMapper Mapper = new MapperConfiguration(config => config.AddProfile(new MappingProfile())).CreateMapper();

        private readonly IBucketRepository repository;

        private readonly IPublisher publisher;

        public Handler(IBucketRepository repository, IPublisher publisher)
        {
            this.repository = repository;
            this.publisher = publisher;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            using Activity? activity = ActivityProvider.StartActivity(nameof(UpdateItem));

            Bucket bucket = await this.repository.GetAsync(request.BucketId, cancellationToken) ??
                throw new BucketNotFoundException(request.BucketId);

            Item? item = bucket.Items.SingleOrDefault(x => x.Id == request.ItemId);

            if (item is null)
            {
                item = Mapper.Map<Item>(request);
                bucket.AddItem(item);

                await this.repository.UpdateAsync(bucket, cancellationToken);
                await this.publisher.Publish(new ItemAddedEvent(bucket.Id, item.Id), CancellationToken.None);

                return new Response(item, true);
            }

            item = Mapper.Map(request, item);

            await this.repository.UpdateAsync(bucket, cancellationToken);
            await this.publisher.Publish(new ItemUpdatedEvent(bucket.Id, item.Id), CancellationToken.None);

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
