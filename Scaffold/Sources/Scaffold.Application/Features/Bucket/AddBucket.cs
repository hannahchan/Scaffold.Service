namespace Scaffold.Application.Features.Bucket
{
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using MediatR;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Domain.Exceptions;

    public static class AddBucket
    {
        public class Command : IRequest<Response>
        {
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
            public Validator() => this.RuleFor(command => command.Name).NotEmpty().NotNull();
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IBucketRepository repository;

            public Handler(IBucketRepository repository) => this.repository = repository;

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                await new Validator().ValidateAndThrowAsync(request);

                Response response = new Response();

                try
                {
                    MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));
                    response.Bucket = configuration.CreateMapper().Map<Bucket>(request);
                }
                catch (AutoMapperMappingException exception) when (exception.InnerException is DomainException)
                {
                    throw exception.InnerException;
                }

                await this.repository.AddAsync(response.Bucket);

                return response;
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
