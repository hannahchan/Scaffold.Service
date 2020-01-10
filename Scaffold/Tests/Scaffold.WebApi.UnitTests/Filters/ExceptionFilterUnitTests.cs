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
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Routing;
    using Scaffold.Application.Base;
    using Scaffold.Domain.Base;
    using Scaffold.WebApi.Filters;
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

                ExceptionFilter exceptionFilter = new ExceptionFilter(new MockProblemDetailsFactory());

                // Act
                exceptionFilter.OnException(context);

                // Assert
                ConflictObjectResult objectResult = Assert.IsType<ConflictObjectResult>(context.Result);
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

                ExceptionFilter exceptionFilter = new ExceptionFilter(new MockProblemDetailsFactory());

                // Act
                exceptionFilter.OnException(context);

                // Assert
                NotFoundObjectResult objectResult = Assert.IsType<NotFoundObjectResult>(context.Result);
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

                ExceptionFilter exceptionFilter = new ExceptionFilter(new MockProblemDetailsFactory());

                // Act
                exceptionFilter.OnException(context);

                // Assert
                BadRequestObjectResult objectResult = Assert.IsType<BadRequestObjectResult>(context.Result);
                ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
                IDictionary<string, string[]> errors = Assert.IsType<Dictionary<string, string[]>>(problemDetails.Extensions["errors"]);

                Assert.Equal(3, errors.Count);
                Assert.Equal(3, errors["property1"].Length);
                Assert.Single(errors["property2"]);
                Assert.Single(errors["property3"]);
            }

            [Fact]
            public void When_HandlingUnhandledException_Expect_NullContextResult()
            {
                // Arrange
                ExceptionContext context = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new Exception(),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(new MockProblemDetailsFactory());

                // Act
                exceptionFilter.OnException(context);

                // Assert
                Assert.Null(context.Result);
            }
        }

        private class MockProblemDetailsFactory : ProblemDetailsFactory
        {
            public override ProblemDetails CreateProblemDetails(
                HttpContext httpContext,
                int? statusCode = null,
                string? title = null,
                string? type = null,
                string? detail = null,
                string? instance = null)
            {
                return new ProblemDetails();
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
                return new ValidationProblemDetails();
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
