namespace Scaffold.Application.Features.Item
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using MediatR;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;

    public static class UpdateItem
    {
        public class Command : IRequest<Response>
        {
            public int BucketId { get; set; }

            public int ItemId { get; set; }

            public string? Name { get; set; }

            public string? Description { get; set; }
        }

        public class Response
        {
            public Response(Item item, bool created = false)
            {
                this.Item = item ?? throw new ArgumentNullException(nameof(item));
                this.Created = created;
            }

            public Item Item { get; private set; }

            public bool Created { get; private set; }

            public bool Updated { get => !this.Created; }
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

            public Handler(IBucketRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                await new Validator().ValidateAndThrowAsync(request);

                Bucket bucket = await this.repository.GetAsync(request.BucketId) ??
                    throw new BucketNotFoundException(request.BucketId);

                Item item = bucket.Items.SingleOrDefault(x => x.Id == request.ItemId);

                MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(new MappingProfile()));

                if (item is null)
                {
                    item = configuration.CreateMapper().Map<Item>(request);
                    bucket.AddItem(item);
                    await this.repository.UpdateAsync(bucket);

                    return new Response(item, true);
                }

                item = configuration.CreateMapper().Map(request, item);
                await this.repository.UpdateAsync(bucket);

                return new Response(item);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                this.CreateMap<Command, Item>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ItemId));
            }
        }
    }
}
