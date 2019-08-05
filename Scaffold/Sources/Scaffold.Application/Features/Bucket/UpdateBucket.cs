namespace Scaffold.Application.Features.Bucket
{
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

            public string Name { get; set; }

            public string Description { get; set; }

            public int? Size { get; set; }
        }

        public class Response
        {
            public Bucket Bucket { get; set; }

            public bool Created { get; set; } = false;

            public bool Updated
            {
                get => !this.Created;

                set => this.Created = !value;
            }
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

            public Handler(IBucketRepository repository) => this.repository = repository;

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                await new Validator().ValidateAndThrowAsync(request);

                Response response = new Response { Bucket = await this.repository.GetAsync(request.Id) };

                try
                {
                    MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));

                    if (response.Bucket == null)
                    {
                        response.Bucket = configuration.CreateMapper().Map<Bucket>(request);
                        await this.repository.AddAsync(response.Bucket);
                        response.Created = true;

                        return response;
                    }

                    response.Bucket = configuration.CreateMapper().Map(request, response.Bucket);
                    await this.repository.UpdateAsync(response.Bucket);

                    return response;
                }
                catch (AutoMapperMappingException exception) when (exception.InnerException is DomainException)
                {
                    throw exception.InnerException;
                }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => this.CreateMap<Command, Bucket>();
        }
    }
}
