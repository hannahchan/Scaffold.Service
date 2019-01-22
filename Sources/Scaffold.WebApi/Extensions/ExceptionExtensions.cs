namespace Scaffold.WebApi.Extensions
{
    using FluentValidation;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Mvc;
    using Scaffold.Application.Exceptions;
    using Scaffold.Domain.Exceptions;

    public static class ExceptionExtensions
    {
        public static ProblemDetails ProblemDetails(this ApplicationException exception)
        {
            return new ProblemDetails
            {
                Title = exception.Title,
                Detail = exception.Detail
            };
        }

        public static ProblemDetails ProblemDetails(this DomainException exception)
        {
            return new ProblemDetails
            {
                Title = exception.Title,
                Detail = exception.Detail
            };
        }

        public static ValidationProblemDetails ProblemDetails(this ValidationException exception)
        {
            ValidationProblemDetails details = new ValidationProblemDetails { Title = "Validation Failure" };

            foreach (ValidationFailure error in exception.Errors)
            {
                details.Errors.Add(error.PropertyName, new[] { error.ErrorMessage });
            }

            return details;
        }
    }
}
