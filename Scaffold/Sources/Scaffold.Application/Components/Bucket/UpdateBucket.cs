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

public static class UpdateBucket
{
    public record Command(int Id, string? Name, string? Description, int? Size) : IRequest<Response>;

    public record Response(Bucket Bucket, bool Created);

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
            using Activity? activity = ActivityProvider.StartActivity(nameof(UpdateBucket));

            Bucket? bucket = await this.repository.GetAsync(request.Id, cancellationToken);

            try
            {
                if (bucket is null)
                {
                    bucket = Mapper.Map<Bucket>(request);

                    await this.repository.AddAsync(bucket, cancellationToken);
                    await this.publisher.Publish(new BucketAddedEvent(bucket.Id), CancellationToken.None);

                    return new Response(bucket, true);
                }

                bucket = Mapper.Map(request, bucket);

                await this.repository.UpdateAsync(bucket, cancellationToken);
                await this.publisher.Publish(new BucketUpdatedEvent(bucket.Id), CancellationToken.None);

                return new Response(bucket, false);
            }
            catch (AutoMapperMappingException exception) when (exception.InnerException is DomainException)
            {
                throw exception.InnerException;
            }
        }
    }

    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Command, Bucket>()
                .ForMember(dest => dest.Size, opt => opt.Condition(src => src.Size != null))
                .ForMember(dest => dest.Items, opt => opt.Ignore());
        }
    }
}
