namespace Scaffold.WebApi.Filters
{
    using FluentValidation;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Scaffold.Application.Exceptions;
    using Scaffold.Domain.Exceptions;
    using Scaffold.WebApi.Views;

    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DomainException domainException)
            {
                context.Result = new ConflictObjectResult(new ErrorResponse(domainException));
            }

            if (context.Exception is NotFoundException notFoundException)
            {
                context.Result = new NotFoundObjectResult(new ErrorResponse(notFoundException));
            }

            if (context.Exception is ValidationException validationException)
            {
                context.Result = new BadRequestObjectResult(new ErrorResponse(validationException));
            }
        }
    }
}
