namespace Scaffold.Application.Features.Bucket
{
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Scaffold.Application.Context;
    using Scaffold.Application.Repositories;
    using Scaffold.Domain.Entities;

    public class AddBucket
    {
        public class Command : ApplicationRequest, IRequest<Response>
        {
            public string Name { get; set; }

            public string Description { get; set; }
        }

        public class Response : ApplicationResponse
        {
            public Bucket Bucket { get; set; }
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

                Response response = new Response
                {
                    Bucket = new Bucket
                    {
                        Name = command.Name,
                        Description = command.Description
                    }
                };

                await this.repository.AddAsync(response.Bucket);

                return response;
            }
        }
    }
}
