namespace Scaffold.Application.Components.Bucket;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Scaffold.Application.Common.Instrumentation;
using Scaffold.Application.Common.Messaging;
using Scaffold.Domain.Aggregates.Bucket;

public static class AddItem
{
    public record Command(int BucketId, string? Name, string? Description) : IRequest<Response>;

    public record Response(Item Item);

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
            using Activity? activity = ActivityProvider.StartActivity(nameof(AddItem));

            Bucket bucket = await this.repository.GetAsync(request.BucketId, cancellationToken) ??
                throw new BucketNotFoundException(request.BucketId);

            Item item = Mapper.Map<Item>(request);

            bucket.AddItem(item);

            await this.repository.UpdateAsync(bucket, cancellationToken);
            await this.publisher.Publish(new ItemAddedEvent(bucket.Id, item.Id), CancellationToken.None);

            return new Response(item);
        }
    }

    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Command, Item>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
