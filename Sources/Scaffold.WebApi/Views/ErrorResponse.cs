namespace Scaffold.WebApi.Views
{
    using System.Collections.Generic;
    using FluentValidation;
    using FluentValidation.Results;
    using Scaffold.Application.Exceptions;
    using Scaffold.Domain.Exceptions;

    public class ErrorResponse
    {
        public ErrorResponse()
        {
        }

        public ErrorResponse(ApplicationException exception)
        {
            this.Errors.Add(new Error { Message = exception.Message });
        }

        public ErrorResponse(DomainException exception)
        {
            this.Errors.Add(new Error { Message = exception.Message });
        }

        public ErrorResponse(ValidationException exception)
        {
            foreach (ValidationFailure error in exception.Errors)
            {
                this.Errors.Add(new Error { Message = error.ErrorMessage });
            }
        }

        public IList<Error> Errors { get; } = new List<Error>();
    }
}
