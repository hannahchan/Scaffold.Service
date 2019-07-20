namespace Scaffold.Application.Features.Item
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

    public static class AddItem
    {
        public class Command : IRequest<Response>
        {
            public int BucketId { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }

        public class Response
        {
            public Item Item { get; set; }
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

                Bucket bucket = await this.repository.GetAsync(request.BucketId) ??
                    throw new BucketNotFoundException(request.BucketId);

                Response response = new Response();

                try
                {
                    MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));
                    response.Item = configuration.CreateMapper().Map<Item>(request);
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
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Bucket, opt => opt.Ignore());
            }
        }
    }
}
