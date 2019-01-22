namespace Scaffold.WebApi.Filters
{
    using FluentValidation;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Scaffold.Application.Exceptions;
    using Scaffold.Domain.Exceptions;
    using Scaffold.WebApi.Extensions;

    public class ExceptionFilter : IExceptionFilter
    {
        private readonly MediaTypeCollection contentTypes = new MediaTypeCollection
        {
            "application/problem+json",
            "application/problem+xml",
        };

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DomainException domainException)
            {
                ProblemDetails details = domainException.ProblemDetails();
                context.Result = new ConflictObjectResult(details) { ContentTypes = this.contentTypes };
            }

            if (context.Exception is NotFoundException notFoundException)
            {
                ProblemDetails details = notFoundException.ProblemDetails();
                context.Result = new NotFoundObjectResult(details) { ContentTypes = this.contentTypes };
            }

            if (context.Exception is ValidationException validationException)
            {
                ValidationProblemDetails details = validationException.ProblemDetails();
                context.Result = new BadRequestObjectResult(details) { ContentTypes = this.contentTypes };
            }
        }
    }
}
