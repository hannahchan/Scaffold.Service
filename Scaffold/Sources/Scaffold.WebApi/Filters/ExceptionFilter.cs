namespace Scaffold.WebApi.Filters
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Scaffold.Application.Base;
    using Scaffold.Domain.Base;
    using Scaffold.WebApi.Extensions;

    public class ExceptionFilter : IActionFilter, IExceptionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult result && result.Value is ProblemDetails details)
            {
                details.AddW3cTraceId();
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Not Used
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DomainException domainException)
            {
                context.Result = new ConflictObjectResult(this.GetProblemDetails(domainException));
            }

            if (context.Exception is NotFoundException notFoundException)
            {
                context.Result = new NotFoundObjectResult(this.GetProblemDetails(notFoundException));
            }

            if (context.Exception is ValidationException validationException)
            {
                context.Result = new BadRequestObjectResult(this.GetProblemDetails(validationException));
            }

            if (context.Result is ObjectResult result && result.Value is ProblemDetails details)
            {
                details.AddW3cTraceId();
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
    }
}
