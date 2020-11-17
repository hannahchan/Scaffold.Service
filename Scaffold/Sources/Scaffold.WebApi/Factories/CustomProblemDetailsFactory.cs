namespace Scaffold.WebApi.Factories
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using OpenTracing;

    public sealed class CustomProblemDetailsFactory : ProblemDetailsFactory
    {
        private readonly ApiBehaviorOptions options;

        public CustomProblemDetailsFactory(IOptions<ApiBehaviorOptions> options)
        {
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public override ProblemDetails CreateProblemDetails(
            HttpContext httpContext,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            statusCode ??= 500;

            ProblemDetails problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance,
            };

            this.ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }

        public override ValidationProblemDetails CreateValidationProblemDetails(
            HttpContext httpContext,
            ModelStateDictionary modelStateDictionary,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (modelStateDictionary is null)
            {
                throw new ArgumentNullException(nameof(modelStateDictionary));
            }

            statusCode ??= 400;

            ValidationProblemDetails problemDetails = new ValidationProblemDetails(modelStateDictionary)
            {
                Status = statusCode,
                Type = type,
                Detail = detail,
                Instance = instance,
            };

            if (title is not null)
            {
                problemDetails.Title = title;
            }

            this.ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }

        private void ApplyProblemDetailsDefaults(HttpContext httpContext, ProblemDetails problemDetails, int statusCode)
        {
            if (this.options.ClientErrorMapping.TryGetValue(statusCode, out ClientErrorData? clientErrorData))
            {
                problemDetails.Title ??= clientErrorData.Title;
                problemDetails.Type ??= clientErrorData.Link;
            }

            ITracer? tracer = httpContext.RequestServices?.GetService<ITracer>();

            if (tracer is not null && tracer.ActiveSpan is ISpan span)
            {
                problemDetails.Extensions["traceId"] = span.Context.TraceId;
            }
        }
    }
}
