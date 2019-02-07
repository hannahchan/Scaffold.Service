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

    public class UpdateBucket
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
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                this.RuleFor(command => command.Id).NotEmpty();
                this.RuleFor(command => command.Name).NotEmpty().When(command => command.Name != null);
            }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IBucketRepository repository;

            public Handler(IBucketRepository repository) => this.repository = repository;

            public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
            {
                Response response = new Response { Bucket = await this.repository.GetAsync(command.Id) };

                if (response.Bucket == null)
                {
                    throw new BucketNotFoundException(command.Id);
                }

                await new Validator().ValidateAndThrowAsync(command);

                try
                {
                    MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));
                    response.Bucket = configuration.CreateMapper().Map<Command, Bucket>(command, response.Bucket);
                }
                catch (AutoMapperMappingException exception) when (exception.InnerException is DomainException)
                {
                    throw exception.InnerException;
                }

                await this.repository.UpdateAsync(response.Bucket);

                return response;
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                this.CreateMap<Command, Bucket>()
                    .AddTransform<string>(value => value == string.Empty ? null : value)
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))
                    .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
                    .ForMember(dest => dest.Size, opt => opt.Condition(src => src.Size != null));
            }
        }
    }
}
