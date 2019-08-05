namespace Scaffold.WebApi.Filters
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Scaffold.Application.Exceptions;
    using Scaffold.Domain.Base;
    using Scaffold.WebApi.Constants;
    using Scaffold.WebApi.Services;

    public class ExceptionFilter : IExceptionFilter
    {
        private readonly RequestTracingService tracingService;

        private readonly MediaTypeCollection contentTypes = new MediaTypeCollection
        {
            CustomMediaTypeNames.Application.ProblemJson,
            CustomMediaTypeNames.Application.ProblemXml,
        };

        public ExceptionFilter(RequestTracingService tracingService) => this.tracingService = tracingService;

        public void OnException(ExceptionContext context)
        {
            ProblemDetails details = null;

            if (context.Exception is DomainException domainException)
            {
                details = this.GetProblemDetails(domainException);
                context.Result = new ConflictObjectResult(details) { ContentTypes = this.contentTypes };
            }

            if (context.Exception is NotFoundException notFoundException)
            {
                details = this.GetProblemDetails(notFoundException);
                context.Result = new NotFoundObjectResult(details) { ContentTypes = this.contentTypes };
            }

            if (context.Exception is ValidationException validationException)
            {
                details = this.GetProblemDetails(validationException);
                context.Result = new BadRequestObjectResult(details) { ContentTypes = this.contentTypes };
            }

            if (details != null)
            {
                if (!string.IsNullOrEmpty(this.tracingService?.CorrelationId))
                {
                    details.Extensions[this.ToCamelCase(CustomHeaderNames.CorrelationId)] = this.tracingService.CorrelationId;
                }

                details.Extensions[this.ToCamelCase(CustomHeaderNames.RequestId)] = context.HttpContext.TraceIdentifier;
            }
        }

        private ProblemDetails GetProblemDetails(ApplicationException exception)
        {
            ProblemDetails details = new ProblemDetails
            {
                Title = exception.Title,
                Detail = exception.Detail,
            };

            return details;
        }

        private ProblemDetails GetProblemDetails(DomainException exception)
        {
            ProblemDetails details = new ProblemDetails
            {
                Title = exception.Title,
                Detail = exception.Detail,
            };

            return details;
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
