namespace Scaffold.WebApi.Filters
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Scaffold.Application.Exceptions;
    using Scaffold.Domain.Exceptions;

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
                ProblemDetails details = this.GetProblemDetails(domainException);
                context.Result = new ConflictObjectResult(details) { ContentTypes = this.contentTypes };
            }

            if (context.Exception is NotFoundException notFoundException)
            {
                ProblemDetails details = this.GetProblemDetails(notFoundException);
                context.Result = new NotFoundObjectResult(details) { ContentTypes = this.contentTypes };
            }

            if (context.Exception is ValidationException validationException)
            {
                ValidationProblemDetails details = this.GetProblemDetails(validationException);
                context.Result = new BadRequestObjectResult(details) { ContentTypes = this.contentTypes };
            }
        }

        private ProblemDetails GetProblemDetails(ApplicationException exception)
        {
            return new ProblemDetails
            {
                Title = exception.Title,
                Detail = exception.Detail
            };
        }

        private ProblemDetails GetProblemDetails(DomainException exception)
        {
            return new ProblemDetails
            {
                Title = exception.Title,
                Detail = exception.Detail
            };
        }

        private ValidationProblemDetails GetProblemDetails(ValidationException exception)
        {
            ValidationProblemDetails details = new ValidationProblemDetails { Title = "Validation Failure" };

            foreach (string property in exception.Errors.Select(error => error.PropertyName).Distinct())
            {
                IEnumerable<string> errorMessages = exception.Errors
                    .Where(error => error.PropertyName == property)
                    .Select(error => error.ErrorMessage)
                    .Distinct();

                details.Errors.Add(property, errorMessages.ToArray());
            }

            return details;
        }
    }
}
