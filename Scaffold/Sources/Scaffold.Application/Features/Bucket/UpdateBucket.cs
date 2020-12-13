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

    public static class UpdateBucket
    {
        public class Command : IRequest<Response>
        {
            public Command(int id, string? name, string? description, int? size)
            {
                this.Id = id;
                this.Name = name;
                this.Description = description;
                this.Size = size;
            }

            public int Id { get; }

            public string? Name { get; }

            public string? Description { get; }

            public int? Size { get; }
        }

        public class Response
        {
            public Response(Bucket bucket, bool created = false)
            {
                this.Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
                this.Created = created;
            }

            public Bucket Bucket { get; }

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
                Bucket? bucket = await this.repository.GetAsync(request.Id, cancellationToken);

                try
                {
                    MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));

                    if (bucket is null)
                    {
                        bucket = configuration.CreateMapper().Map<Bucket>(request);
                        await this.repository.AddAsync(bucket, cancellationToken);

                        return new Response(bucket, true);
                    }

                    bucket = configuration.CreateMapper().Map(request, bucket);
                    await this.repository.UpdateAsync(bucket, cancellationToken);

                    return new Response(bucket);
                }
                catch (AutoMapperMappingException exception) when (exception.InnerException is DomainException)
                {
                    throw exception.InnerException;
                }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                this.CreateMap<Command, Bucket>()
                    .ForMember(dest => dest.Size, opt => opt.Condition(src => src.Size != null))
                    .ForMember(dest => dest.Items, opt => opt.Ignore());
            }
        }
    }
}
