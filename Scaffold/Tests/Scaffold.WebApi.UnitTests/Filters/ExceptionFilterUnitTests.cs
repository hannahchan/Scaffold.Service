namespace Scaffold.WebApi.UnitTests.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using FluentValidation;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Scaffold.Application.Exceptions;
    using Scaffold.Domain.Base;
    using Scaffold.WebApi.Filters;
    using Scaffold.WebApi.Services;
    using Xunit;

    public class ExceptionFilterUnitTests
    {
        private readonly ActionContext actionContext;

        public ExceptionFilterUnitTests()
        {
            this.actionContext = new ActionContext
            {
                ActionDescriptor = new ActionDescriptor(),
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
            };
        }

        public class OnActionExecuted : ExceptionFilterUnitTests
        {
            [Fact]
            public void When_HandlingObjectResultWithProblemDetailsWithNullRequestTracingService_Expect_ProblemDetailsWithRequestId()
            {
                // Arrange
                ActionExecutedContext context = new ActionExecutedContext(this.actionContext, new List<IFilterMetadata>(), null)
                {
                    Result = new ObjectResult(new ProblemDetails()),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(null);

                // Act
                exceptionFilter.OnActionExecuted(context);

                // Assert
                ObjectResult objectResult = context.Result as ObjectResult;
                ProblemDetails problemDetails = objectResult.Value as ProblemDetails;
                Assert.NotNull(Record.Exception(() => problemDetails.Extensions["correlation-Id"]));
                Assert.Equal(context.HttpContext.TraceIdentifier, problemDetails.Extensions["request-Id"]);
            }

            [Fact]
            public void When_HandlingObjectResultWithProblemDetailsWithNullCorrelationId_Expect_ProblemDetailsWithRequestId()
            {
                // Arrange
                ActionExecutedContext context = new ActionExecutedContext(this.actionContext, new List<IFilterMetadata>(), null)
                {
                    Result = new ObjectResult(new ProblemDetails()),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(new RequestTracingService());

                // Act
                exceptionFilter.OnActionExecuted(context);

                // Assert
                ObjectResult objectResult = context.Result as ObjectResult;
                ProblemDetails problemDetails = objectResult.Value as ProblemDetails;
                Assert.NotNull(Record.Exception(() => problemDetails.Extensions["correlation-Id"]));
                Assert.Equal(context.HttpContext.TraceIdentifier, problemDetails.Extensions["request-Id"]);
            }

            [Fact]
            public void When_HandlingObjectResultWithProblemDetailsWithCorrelationId_Expect_ProblemDetailsWithCorrelationIdAndRequestId()
            {
                // Arrange
                ActionExecutedContext context = new ActionExecutedContext(this.actionContext, new List<IFilterMetadata>(), null)
                {
                    Result = new ObjectResult(new ProblemDetails()),
                };

                string correlationId = Guid.NewGuid().ToString();

                ExceptionFilter exceptionFilter = new ExceptionFilter(new RequestTracingService
                {
                    CorrelationId = correlationId,
                });

                // Act
                exceptionFilter.OnActionExecuted(context);

                // Assert
                ObjectResult objectResult = context.Result as ObjectResult;
                ProblemDetails problemDetails = objectResult.Value as ProblemDetails;
                Assert.Equal(correlationId, problemDetails.Extensions["correlation-Id"]);
                Assert.Equal(context.HttpContext.TraceIdentifier, problemDetails.Extensions["request-Id"]);
            }
        }

        public class OnActionExecuting : ExceptionFilterUnitTests
        {
            [Fact]
            public void When_ActionExecuting_Expect_NullContextResult()
            {
                // Arrange
                ActionExecutingContext context = new ActionExecutingContext(
                    this.actionContext,
                    new List<IFilterMetadata>(),
                    new Dictionary<string, object>(),
                    null);

                ExceptionFilter exceptionFilter = new ExceptionFilter(null);

                // Act
                exceptionFilter.OnActionExecuting(context);

                // Assert
                Assert.Empty(context.ActionArguments);
                Assert.Null(context.Controller);
                Assert.Empty(context.Filters);
                Assert.Null(context.Result);
            }
        }

        public class OnException : ExceptionFilterUnitTests
        {
            [Fact]
            public void When_HandlingDomainException_Expect_ConflictObjectResult()
            {
                // Arrange
                ExceptionContext context = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new TestDomainException(Guid.NewGuid().ToString()),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(null);

                // Act
                exceptionFilter.OnException(context);

                // Assert
                Assert.IsType<ConflictObjectResult>(context.Result);

                ConflictObjectResult objectResult = context.Result as ConflictObjectResult;
                Assert.IsType<ProblemDetails>(objectResult.Value);
            }

            [Fact]
            public void When_HandlingNotFoundException_Expect_NotFoundObjectResult()
            {
                // Arrange
                ExceptionContext context = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new TestNotFoundException(Guid.NewGuid().ToString()),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(null);

                // Act
                exceptionFilter.OnException(context);

                // Assert
                Assert.IsType<NotFoundObjectResult>(context.Result);

                NotFoundObjectResult objectResult = context.Result as NotFoundObjectResult;
                Assert.IsType<ProblemDetails>(objectResult.Value);
            }

            [Fact]
            public void When_HandlingValidationException_Expect_BadRequestObjectResult()
            {
                // Arrange
                List<ValidationFailure> validationFailures = new List<ValidationFailure>
                {
                    new ValidationFailure("property1", "Error Message."),
                    new ValidationFailure("property1", "Another Error Message."),
                    new ValidationFailure("property1", "One More Error Message."),
                    new ValidationFailure("property2", "Error Message."),
                    new ValidationFailure("property3", "Error Message."),
                };

                ExceptionContext context = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new ValidationException(Guid.NewGuid().ToString(), validationFailures),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(null);

                // Act
                exceptionFilter.OnException(context);

                // Assert
                Assert.IsType<BadRequestObjectResult>(context.Result);

                BadRequestObjectResult objectResult = context.Result as BadRequestObjectResult;
                Assert.IsType<ValidationProblemDetails>(objectResult.Value);

                ValidationProblemDetails problemDetails = objectResult.Value as ValidationProblemDetails;
                Assert.Equal(3, problemDetails.Errors.Count);
                Assert.Equal(3, problemDetails.Errors["property1"].Length);
                Assert.Single(problemDetails.Errors["property2"]);
                Assert.Single(problemDetails.Errors["property3"]);
            }

            [Fact]
            public void When_HandlingUnhandledException_Expect_NullContextResult()
            {
                // Arrange
                ExceptionContext context = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new Exception(),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(null);

                // Act
                exceptionFilter.OnException(context);

                // Assert
                Assert.Null(context.Result);
            }

            [Fact]
            public void When_HandlingExceptionWithNullRequestTracingService_Expect_ProblemDetailsWithRequestId()
            {
                // Arrange
                ExceptionContext context = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new TestDomainException(Guid.NewGuid().ToString()),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(null);

                // Act
                exceptionFilter.OnException(context);

                // Assert
                ObjectResult objectResult = context.Result as ObjectResult;
                ProblemDetails problemDetails = objectResult.Value as ProblemDetails;
                Assert.NotNull(Record.Exception(() => problemDetails.Extensions["correlation-Id"]));
                Assert.Equal(context.HttpContext.TraceIdentifier, problemDetails.Extensions["request-Id"]);
            }

            [Fact]
            public void When_HandlingExceptionWithNullCorrelationId_Expect_ProblemDetailsWithRequestId()
            {
                // Arrange
                ExceptionContext context = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new TestDomainException(Guid.NewGuid().ToString()),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(new RequestTracingService());

                // Act
                exceptionFilter.OnException(context);

                // Assert
                ObjectResult objectResult = context.Result as ObjectResult;
                ProblemDetails problemDetails = objectResult.Value as ProblemDetails;
                Assert.NotNull(Record.Exception(() => problemDetails.Extensions["correlation-Id"]));
                Assert.Equal(context.HttpContext.TraceIdentifier, problemDetails.Extensions["request-Id"]);
            }

            [Fact]
            public void When_HandlingExceptionWithCorrelationId_Expect_ProblemDetailsWithCorrelationIdAndRequestId()
            {
                // Arrange
                ExceptionContext context = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new TestDomainException(Guid.NewGuid().ToString()),
                };

                string correlationId = Guid.NewGuid().ToString();

                ExceptionFilter exceptionFilter = new ExceptionFilter(new RequestTracingService
                {
                    CorrelationId = correlationId,
                });

                // Act
                exceptionFilter.OnException(context);

                // Assert
                ObjectResult objectResult = context.Result as ObjectResult;
                ProblemDetails problemDetails = objectResult.Value as ProblemDetails;
                Assert.Equal(correlationId, problemDetails.Extensions["correlation-Id"]);
                Assert.Equal(context.HttpContext.TraceIdentifier, problemDetails.Extensions["request-Id"]);
            }
        }

        [Serializable]
        private class TestDomainException : DomainException
        {
            public TestDomainException(string message)
                : base(message)
            {
            }

            public TestDomainException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            public TestDomainException(string title, string message)
                : base(title, message)
            {
            }

            public TestDomainException(string title, string message, Exception innerException)
                : base(title, message, innerException)
            {
            }

            protected TestDomainException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }

        [Serializable]
        private class TestNotFoundException : NotFoundException
        {
            public TestNotFoundException(string message)
                : base(message)
            {
            }

            public TestNotFoundException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            public TestNotFoundException(string title, string message)
                : base(title, message)
            {
            }

            public TestNotFoundException(string title, string message, Exception innerException)
                : base(title, message, innerException)
            {
            }

            protected TestNotFoundException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}
