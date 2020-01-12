namespace Scaffold.WebApi.Filters
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Scaffold.Application.Base;
    using Scaffold.Domain.Base;

    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ProblemDetailsFactory problemDetailsFactory;

        public ExceptionFilter(ProblemDetailsFactory problemDetailsFactory)
        {
            this.problemDetailsFactory = problemDetailsFactory;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DomainException domainException)
            {
                context.Result = new ConflictObjectResult(this.problemDetailsFactory.CreateProblemDetails(
                    httpContext: context.HttpContext,
                    statusCode: (int)HttpStatusCode.Conflict,
                    title: domainException.Title,
                    detail: domainException.Detail));
            }

            if (context.Exception is NotFoundException notFoundException)
            {
                context.Result = new NotFoundObjectResult(this.problemDetailsFactory.CreateProblemDetails(
                    httpContext: context.HttpContext,
                    statusCode: (int)HttpStatusCode.NotFound,
                    title: notFoundException.Title,
                    detail: notFoundException.Detail));
            }
        }
    }
}
