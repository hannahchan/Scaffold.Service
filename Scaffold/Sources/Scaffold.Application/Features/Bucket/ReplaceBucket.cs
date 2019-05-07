namespace Scaffold.Application.Features.Bucket
{
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using MediatR;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Entities;
    using Scaffold.Domain.Exceptions;

    public class ReplaceBucket
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

            public bool Replaced
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

            public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
            {
                await new Validator().ValidateAndThrowAsync(command);

                Response response = new Response { Bucket = await this.repository.GetAsync(command.Id) };

                try
                {
                    MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));

                    if (response.Bucket == null)
                    {
                        response.Bucket = configuration.CreateMapper().Map<Bucket>(command);
                        await this.repository.AddAsync(response.Bucket);
                        response.Created = true;

                        return response;
                    }

                    response.Bucket = configuration.CreateMapper().Map<Command, Bucket>(command, response.Bucket);
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
            public MappingProfile()
            {
                this.CreateMap<Command, Bucket>()
                    .AddTransform<string>(value => value == string.Empty ? null : value);
            }
        }
    }
}
