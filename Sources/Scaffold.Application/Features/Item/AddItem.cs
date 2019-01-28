namespace Scaffold.Application.Features.Item
{
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using MediatR;
    using Scaffold.Application.Context;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Repositories;
    using Scaffold.Domain.Entities;
    using Scaffold.Domain.Exceptions;

    public class AddItem
    {
        public class Command : ApplicationRequest, IRequest<Response>
        {
            public int BucketId { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }

        public class Response : ApplicationResponse
        {
            public Item Item { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
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

                Bucket bucket = await this.repository.GetAsync(command.BucketId) ??
                    throw new BucketNotFoundException(command.BucketId);

                Response response = new Response();

                try
                {
                    MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));
                    response.Item = configuration.CreateMapper().Map<Item>(command);
                }
                catch (AutoMapperMappingException exception) when (exception.InnerException is DomainException)
                {
                    throw exception.InnerException;
                }

                bucket.AddItem(response.Item);

                await this.repository.UpdateAsync(bucket);

                return response;
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                this.CreateMap<Command, Item>()
                    .AddTransform<string>(value => value == string.Empty ? null : value)
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Bucket, opt => opt.Ignore());
            }
        }
    }
}
