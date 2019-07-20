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

    public static class UpdateItem
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
                this.RuleFor(command => command.BucketId).NotEmpty();
                this.RuleFor(command => command.ItemId).NotEmpty();
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

                Bucket bucket = await this.repository.GetAsync(request.BucketId) ??
                    throw new BucketNotFoundException(request.BucketId);

                Response response = new Response { Item = bucket.Items.SingleOrDefault(x => x.Id == request.ItemId) };

                try
                {
                    MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));

                    if (response.Item == null)
                    {
                        response.Item = configuration.CreateMapper().Map<Item>(request);
                        bucket.AddItem(response.Item);
                        await this.repository.UpdateAsync(bucket);
                        response.Created = true;

                        return response;
                    }

                    response.Item = configuration.CreateMapper().Map(request, response.Item);
                    await this.repository.UpdateAsync(bucket);

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
                this.CreateMap<Command, Item>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ItemId))
                    .ForMember(dest => dest.Bucket, opt => opt.Ignore());
            }
        }
    }
}
