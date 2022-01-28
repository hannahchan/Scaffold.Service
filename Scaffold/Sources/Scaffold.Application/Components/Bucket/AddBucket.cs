namespace Scaffold.Application.Components.Bucket;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Scaffold.Application.Common.Instrumentation;
using Scaffold.Application.Common.Messaging;
using Scaffold.Domain.Aggregates.Bucket;
using Scaffold.Domain.Base;

public static class AddBucket
{
    public record Command(string? Name, string? Description, int? Size) : IRequest<Response>;

    public record Response(Bucket Bucket);

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
            using Activity? activity = ActivityProvider.StartActivity(nameof(AddBucket));

            Bucket bucket;

            try
            {
                bucket = Mapper.Map<Bucket>(request);
            }
            catch (AutoMapperMappingException exception) when (exception.InnerException is DomainException)
            {
                throw exception.InnerException;
            }

            await this.repository.AddAsync(bucket, cancellationToken);
            await this.publisher.Publish(new BucketAddedEvent(bucket.Id), CancellationToken.None);

            return new Response(bucket);
        }
    }

    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Command, Bucket>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Size, opt => opt.Condition(src => src.Size != null))
                .ForMember(dest => dest.Items, opt => opt.Ignore());
        }
    }
}
