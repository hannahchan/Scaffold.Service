namespace Scaffold.Application.Features.Item
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using MediatR;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Entities;
    using Scaffold.Domain.Exceptions;

    public class AddItem
    {
        public class Command : IRequest<Response>
        {
            public int BucketId { get; set; }

            public int ItemId { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }

        public class Response
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

                if (bucket.Items.Any(x => x.Id == command.ItemId))
                {
                    throw new DuplicateIdException($"An item with the same Id. '{command.ItemId}' has already been added.");
                }

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
                    .ForMember(dest => dest.Bucket, opt => opt.Ignore())
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ItemId));
            }
        }
    }
}
