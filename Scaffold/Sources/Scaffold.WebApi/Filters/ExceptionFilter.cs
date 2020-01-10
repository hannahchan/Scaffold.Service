namespace Scaffold.WebApi.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Scaffold.Application.Base;
    using Scaffold.Domain.Base;

    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ProblemDetailsFactory factory;

        public ExceptionFilter(ProblemDetailsFactory factory)
        {
            this.factory = factory;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DomainException domainException)
            {
                context.Result = new ConflictObjectResult(this.factory.CreateProblemDetails(
                    context.HttpContext,
                    title: domainException.Title,
                    detail: domainException.Detail));
            }

            if (context.Exception is NotFoundException notFoundException)
            {
                context.Result = new NotFoundObjectResult(this.factory.CreateProblemDetails(
                    context.HttpContext,
                    title: notFoundException.Title,
                    detail: notFoundException.Detail));
            }
        }
    }
}
