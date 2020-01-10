namespace Scaffold.Application.Features.Bucket
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Domain.Base;

    public static class AddBucket
    {
        public class Command : IRequest<Response>
        {
            public string? Name { get; set; }

            public string? Description { get; set; }

            public int? Size { get; set; }
        }

        public class Response
        {
            public Response(Bucket bucket)
            {
                this.Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
            }

            public Bucket Bucket { get; private set; }
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
                Bucket bucket;

                try
                {
                    MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));
                    bucket = configuration.CreateMapper().Map<Bucket>(request);
                }
                catch (AutoMapperMappingException exception) when (exception.InnerException is DomainException)
                {
                    throw exception.InnerException;
                }

                await this.repository.AddAsync(bucket);

                return new Response(bucket);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                this.CreateMap<Command, Bucket>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Size, opt => opt.Condition(src => src.Size != null));
            }
        }
    }
}
