namespace Scaffold.WebApi.UnitTests.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Runtime.Serialization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Routing;
    using Scaffold.Application.Common.Exceptions;
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
                TestDomainException exception = new TestDomainException(Guid.NewGuid().ToString());

                ExceptionContext context = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = exception,
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(new MockProblemDetailsFactory());

                // Act
                exceptionFilter.OnException(context);

                // Assert
                ConflictObjectResult objectResult = Assert.IsType<ConflictObjectResult>(context.Result);
                ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);

                Assert.Equal((int)HttpStatusCode.Conflict, problemDetails.Status);
                Assert.Equal(exception.Message, problemDetails.Detail);
            }

            [Fact]
            public void When_HandlingNotFoundException_Expect_NotFoundObjectResult()
            {
                // Arrange
                TestNotFoundException exception = new TestNotFoundException(Guid.NewGuid().ToString());

                ExceptionContext context = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = exception,
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(new MockProblemDetailsFactory());

                // Act
                exceptionFilter.OnException(context);

                // Assert
                NotFoundObjectResult objectResult = Assert.IsType<NotFoundObjectResult>(context.Result);
                ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);

                Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
                Assert.Equal(exception.Message, problemDetails.Detail);
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
                string title = null,
                string type = null,
                string detail = null,
                string instance = null)
            {
                return new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Type = type,
                    Detail = detail,
                    Instance = instance,
                };
            }

            public override ValidationProblemDetails CreateValidationProblemDetails(
                HttpContext httpContext,
                ModelStateDictionary modelStateDictionary,
                int? statusCode = null,
                string title = null,
                string type = null,
                string detail = null,
                string instance = null)
            {
                return new ValidationProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Type = type,
                    Detail = detail,
                    Instance = instance,
                };
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

            protected TestNotFoundException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}
