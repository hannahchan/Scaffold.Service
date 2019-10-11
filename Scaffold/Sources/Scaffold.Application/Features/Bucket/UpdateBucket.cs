namespace Scaffold.Application.Features.Bucket
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using MediatR;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Domain.Base;

    public static class UpdateBucket
    {
        public class Command : IRequest<Response>
        {
            public int Id { get; set; }

            public string? Name { get; set; }

            public string? Description { get; set; }

            public int? Size { get; set; }
        }

        public class Response
        {
            public Response(Bucket bucket, bool created = false)
            {
                this.Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
                this.Created = created;
            }

            public Bucket Bucket { get; private set; }

            public bool Created { get; private set; }

            public bool Updated { get => !this.Created; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                this.RuleFor(command => command.Id).NotEmpty();
                this.RuleFor(command => command.Name).NotEmpty().NotNull();
            }
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
                await new Validator().ValidateAndThrowAsync(request);

                Bucket bucket = await this.repository.GetAsync(request.Id);

                try
                {
                    MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));

                    if (bucket is null)
                    {
                        bucket = configuration.CreateMapper().Map<Bucket>(request);
                        await this.repository.AddAsync(bucket);

                        return new Response(bucket, true);
                    }

                    bucket = configuration.CreateMapper().Map(request, bucket);
                    await this.repository.UpdateAsync(bucket);

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
                this.CreateMap<Command, Bucket>();
            }
        }
    }
}
