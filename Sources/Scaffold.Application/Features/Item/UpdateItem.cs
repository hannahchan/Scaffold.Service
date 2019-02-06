namespace Scaffold.Application.Features.Item
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using MediatR;
    using Scaffold.Application.Context;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Interfaces;
    using Scaffold.Application.Repositories;
    using Scaffold.Domain.Entities;
    using Scaffold.Domain.Exceptions;

    public class UpdateItem
    {
        public class Command : ApplicationRequest, IRequest<Response>
        {
            public int BucketId { get; set; }

            public int ItemId { get; set; }

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
                this.RuleFor(command => command.BucketId).NotEmpty();
                this.RuleFor(command => command.ItemId).NotEmpty();
                this.RuleFor(command => command.Name).NotEmpty().When(command => command.Name != null);
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

                Item item = bucket.Items.SingleOrDefault(x => x.Id == command.ItemId) ??
                    throw new ItemNotFoundException(command.ItemId);

                try
                {
                    MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));
                    item = configuration.CreateMapper().Map<Command, Item>(command, item);
                }
                catch (AutoMapperMappingException exception) when (exception.InnerException is DomainException)
                {
                    throw exception.InnerException;
                }

                await this.repository.UpdateAsync(bucket);

                return new Response { Item = item };
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                this.CreateMap<Command, Item>()
                    .AddTransform<string>(value => value == string.Empty ? null : value)
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))
                    .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
                    .ForMember(dest => dest.Bucket,  opt => opt.Ignore());
            }
        }
    }
}
