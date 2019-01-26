namespace Scaffold.Application.Features.Item
{
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Scaffold.Application.Context;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Repositories;
    using Scaffold.Domain.Entities;

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

                Response response = new Response
                {
                    Item = new Item
                    {
                        Name = command.Name,
                        Description = command.Description
                    }
                };

                bucket.AddItem(response.Item);

                await this.repository.UpdateAsync(bucket);

                return response;
            }
        }
    }
}
