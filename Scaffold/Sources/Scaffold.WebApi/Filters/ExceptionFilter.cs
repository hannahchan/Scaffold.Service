namespace Scaffold.WebApi.Filters
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Exceptions;
    using Scaffold.WebApi.Constants;

    public class ExceptionFilter : IExceptionFilter
    {
        private readonly IRequestTracingService tracingService;

        private readonly MediaTypeCollection contentTypes = new MediaTypeCollection
        {
            "application/problem+json",
            "application/problem+xml",
        };

        public ExceptionFilter(IRequestTracingService tracingService) => this.tracingService = tracingService;

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DomainException domainException)
            {
                ProblemDetails details = this.GetProblemDetails(domainException);
                details.Extensions[this.ToCamelCase(Headers.RequestId)] = context.HttpContext.TraceIdentifier;
                context.Result = new ConflictObjectResult(details) { ContentTypes = this.contentTypes };
            }

            if (context.Exception is NotFoundException notFoundException)
            {
                ProblemDetails details = this.GetProblemDetails(notFoundException);
                details.Extensions[this.ToCamelCase(Headers.RequestId)] = context.HttpContext.TraceIdentifier;
                context.Result = new NotFoundObjectResult(details) { ContentTypes = this.contentTypes };
            }

            if (context.Exception is ValidationException validationException)
            {
                ValidationProblemDetails details = this.GetProblemDetails(validationException);
                details.Extensions[this.ToCamelCase(Headers.RequestId)] = context.HttpContext.TraceIdentifier;
                context.Result = new BadRequestObjectResult(details) { ContentTypes = this.contentTypes };
            }
        }

        private ProblemDetails GetProblemDetails(ApplicationException exception)
        {
            ProblemDetails details = new ProblemDetails
            {
                Title = exception.Title,
                Detail = exception.Detail
            };

            if (!string.IsNullOrEmpty(this.tracingService?.CorrelationId))
            {
                details.Extensions[this.ToCamelCase(Headers.CorrelationId)] = this.tracingService.CorrelationId;
            }

            return details;
        }

        private ProblemDetails GetProblemDetails(DomainException exception)
        {
            ProblemDetails details = new ProblemDetails
            {
                Title = exception.Title,
                Detail = exception.Detail
            };

            if (!string.IsNullOrEmpty(this.tracingService?.CorrelationId))
            {
                details.Extensions[this.ToCamelCase(Headers.CorrelationId)] = this.tracingService.CorrelationId;
            }

            return details;
        }

        private ValidationProblemDetails GetProblemDetails(ValidationException exception)
        {
            ValidationProblemDetails details = new ValidationProblemDetails { Title = "Validation Failure" };

            if (!string.IsNullOrEmpty(this.tracingService?.CorrelationId))
            {
                details.Extensions[this.ToCamelCase(Headers.CorrelationId)] = this.tracingService.CorrelationId;
            }

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

        private string ToCamelCase(string text)
        {
            text = text.Replace(" ", string.Empty);
            text = text.Replace("-", string.Empty);
            text = text.Replace(".", string.Empty);
            text = text.Replace("_", string.Empty);
            return $"{char.ToLowerInvariant(text[0])}{text.Substring(1)}";
        }
    }
}
