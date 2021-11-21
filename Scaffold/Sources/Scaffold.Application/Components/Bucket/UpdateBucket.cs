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
                MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));

                if (bucket is null)
                {
                    bucket = configuration.CreateMapper().Map<Bucket>(request);

                    await this.repository.AddAsync(bucket, cancellationToken);
                    await this.publisher.Publish(new BucketAddedEvent<Handler>(bucket.Id), CancellationToken.None);

                    return new Response(bucket, true);
                }

                bucket = configuration.CreateMapper().Map(request, bucket);

                await this.repository.UpdateAsync(bucket, cancellationToken);
                await this.publisher.Publish(new BucketUpdatedEvent<Handler>(bucket.Id), CancellationToken.None);

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
