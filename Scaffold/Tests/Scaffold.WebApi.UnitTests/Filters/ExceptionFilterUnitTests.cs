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

        public class OnDomainException : ExceptionFilterUnitTests
        {
            [Fact]
            public void When_HandlingDomainExceptionWithNullRequestTracingService_Expect_ConflictObjectResult()
            {
                // Arrange
                ExceptionContext exceptionContext = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new TestDomainException(Guid.NewGuid().ToString()),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(null);

                // Act
                exceptionFilter.OnException(exceptionContext);

                // Assert
                Assert.IsType<ConflictObjectResult>(exceptionContext.Result);

                ConflictObjectResult objectResult = exceptionContext.Result as ConflictObjectResult;
                Assert.IsType<ProblemDetails>(objectResult.Value);

                ProblemDetails problemDetails = objectResult.Value as ProblemDetails;
                Assert.NotNull(Record.Exception(() => problemDetails.Extensions["correlationId"]));
                Assert.Equal(exceptionContext.HttpContext.TraceIdentifier, problemDetails.Extensions["requestId"]);
            }

            [Fact]
            public void When_HandlingDomainExceptionWithNullCorrelationId_Expect_ConflictObjectResult()
            {
                // Arrange
                ExceptionContext exceptionContext = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new TestDomainException(Guid.NewGuid().ToString()),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(new RequestTracingService());

                // Act
                exceptionFilter.OnException(exceptionContext);

                // Assert
                Assert.IsType<ConflictObjectResult>(exceptionContext.Result);

                ConflictObjectResult objectResult = exceptionContext.Result as ConflictObjectResult;
                Assert.IsType<ProblemDetails>(objectResult.Value);

                ProblemDetails problemDetails = objectResult.Value as ProblemDetails;
                Assert.NotNull(Record.Exception(() => problemDetails.Extensions["correlationId"]));
                Assert.Equal(exceptionContext.HttpContext.TraceIdentifier, problemDetails.Extensions["requestId"]);
            }

            [Fact]
            public void When_HandlingDomainExceptionWithCorrelationId_Expect_ConflictObjectResult()
            {
                // Arrange
                ExceptionContext exceptionContext = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new TestDomainException(Guid.NewGuid().ToString()),
                };

                string correlationId = Guid.NewGuid().ToString();

                ExceptionFilter exceptionFilter = new ExceptionFilter(new RequestTracingService
                {
                    CorrelationId = correlationId,
                });

                // Act
                exceptionFilter.OnException(exceptionContext);

                // Assert
                Assert.IsType<ConflictObjectResult>(exceptionContext.Result);

                ConflictObjectResult objectResult = exceptionContext.Result as ConflictObjectResult;
                Assert.IsType<ProblemDetails>(objectResult.Value);

                ProblemDetails problemDetails = objectResult.Value as ProblemDetails;
                Assert.Equal(correlationId, problemDetails.Extensions["correlationId"]);
                Assert.Equal(exceptionContext.HttpContext.TraceIdentifier, problemDetails.Extensions["requestId"]);
            }
        }

        public class OnNotFoundException : ExceptionFilterUnitTests
        {
            [Fact]
            public void When_HandlingNotFoundExceptionWithNullRequestTracingService_Expect_NotFoundObjectResult()
            {
                // Arrange
                ExceptionContext exceptionContext = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new TestNotFoundException(Guid.NewGuid().ToString()),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(null);

                // Act
                exceptionFilter.OnException(exceptionContext);

                // Assert
                Assert.IsType<NotFoundObjectResult>(exceptionContext.Result);

                NotFoundObjectResult objectResult = exceptionContext.Result as NotFoundObjectResult;
                Assert.IsType<ProblemDetails>(objectResult.Value);

                ProblemDetails problemDetails = objectResult.Value as ProblemDetails;
                Assert.NotNull(Record.Exception(() => problemDetails.Extensions["correlationId"]));
                Assert.Equal(exceptionContext.HttpContext.TraceIdentifier, problemDetails.Extensions["requestId"]);
            }

            [Fact]
            public void When_HandlingNotFoundExceptionWithNullCorrelationId_Expect_NotFoundObjectResult()
            {
                // Arrange
                ExceptionContext exceptionContext = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new TestNotFoundException(Guid.NewGuid().ToString()),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(new RequestTracingService());

                // Act
                exceptionFilter.OnException(exceptionContext);

                // Assert
                Assert.IsType<NotFoundObjectResult>(exceptionContext.Result);

                NotFoundObjectResult objectResult = exceptionContext.Result as NotFoundObjectResult;
                Assert.IsType<ProblemDetails>(objectResult.Value);

                ProblemDetails problemDetails = objectResult.Value as ProblemDetails;
                Assert.NotNull(Record.Exception(() => problemDetails.Extensions["correlationId"]));
                Assert.Equal(exceptionContext.HttpContext.TraceIdentifier, problemDetails.Extensions["requestId"]);
            }

            [Fact]
            public void When_HandlingNotFoundExceptionWithCorrelationId_Expect_NotFoundObjectResult()
            {
                // Arrange
                ExceptionContext exceptionContext = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new TestNotFoundException(Guid.NewGuid().ToString()),
                };

                string correlationId = Guid.NewGuid().ToString();

                ExceptionFilter exceptionFilter = new ExceptionFilter(new RequestTracingService
                {
                    CorrelationId = correlationId,
                });

                // Act
                exceptionFilter.OnException(exceptionContext);

                // Assert
                Assert.IsType<NotFoundObjectResult>(exceptionContext.Result);

                NotFoundObjectResult objectResult = exceptionContext.Result as NotFoundObjectResult;
                Assert.IsType<ProblemDetails>(objectResult.Value);

                ProblemDetails problemDetails = objectResult.Value as ProblemDetails;
                Assert.Equal(correlationId, problemDetails.Extensions["correlationId"]);
                Assert.Equal(exceptionContext.HttpContext.TraceIdentifier, problemDetails.Extensions["requestId"]);
            }
        }

        public class OnValidationException : ExceptionFilterUnitTests
        {
            [Fact]
            public void When_HandlingValidationExceptionWithNullRequestTracingService_Expect_BadRequestObjectResult()
            {
                // Arrange
                ExceptionContext exceptionContext = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new ValidationException(Guid.NewGuid().ToString()),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(null);

                // Act
                exceptionFilter.OnException(exceptionContext);

                // Assert
                Assert.IsType<BadRequestObjectResult>(exceptionContext.Result);

                BadRequestObjectResult objectResult = exceptionContext.Result as BadRequestObjectResult;
                Assert.IsType<ValidationProblemDetails>(objectResult.Value);

                ValidationProblemDetails problemDetails = objectResult.Value as ValidationProblemDetails;
                Assert.NotNull(Record.Exception(() => problemDetails.Extensions["correlationId"]));
                Assert.Equal(exceptionContext.HttpContext.TraceIdentifier, problemDetails.Extensions["requestId"]);
            }

            [Fact]
            public void When_HandlingValidationExceptionWithNullCorrelationId_Expect_BadRequestObjectResult()
            {
                // Arrange
                ExceptionContext exceptionContext = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new ValidationException(Guid.NewGuid().ToString()),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(new RequestTracingService());

                // Act
                exceptionFilter.OnException(exceptionContext);

                // Assert
                Assert.IsType<BadRequestObjectResult>(exceptionContext.Result);

                BadRequestObjectResult objectResult = exceptionContext.Result as BadRequestObjectResult;
                Assert.IsType<ValidationProblemDetails>(objectResult.Value);

                ValidationProblemDetails problemDetails = objectResult.Value as ValidationProblemDetails;
                Assert.NotNull(Record.Exception(() => problemDetails.Extensions["correlationId"]));
                Assert.Equal(exceptionContext.HttpContext.TraceIdentifier, problemDetails.Extensions["requestId"]);
            }

            [Fact]
            public void When_HandlingValidationExceptionWithCorrelationId_Expect_BadRequestObjectResult()
            {
                // Arrange
                ExceptionContext exceptionContext = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new ValidationException(Guid.NewGuid().ToString()),
                };

                string correlationId = Guid.NewGuid().ToString();

                ExceptionFilter exceptionFilter = new ExceptionFilter(new RequestTracingService
                {
                    CorrelationId = correlationId,
                });

                // Act
                exceptionFilter.OnException(exceptionContext);

                // Assert
                Assert.IsType<BadRequestObjectResult>(exceptionContext.Result);

                BadRequestObjectResult objectResult = exceptionContext.Result as BadRequestObjectResult;
                Assert.IsType<ValidationProblemDetails>(objectResult.Value);

                ValidationProblemDetails problemDetails = objectResult.Value as ValidationProblemDetails;
                Assert.Equal(correlationId, problemDetails.Extensions["correlationId"]);
                Assert.Equal(exceptionContext.HttpContext.TraceIdentifier, problemDetails.Extensions["requestId"]);
            }

            [Fact]
            public void When_HandlingValidationException_Expect_ValidationProblemDetailsWithErrors()
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

                ExceptionContext exceptionContext = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new ValidationException(Guid.NewGuid().ToString(), validationFailures),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(null);

                // Act
                exceptionFilter.OnException(exceptionContext);

                // Assert
                Assert.IsType<BadRequestObjectResult>(exceptionContext.Result);

                BadRequestObjectResult objectResult = exceptionContext.Result as BadRequestObjectResult;
                Assert.IsType<ValidationProblemDetails>(objectResult.Value);

                ValidationProblemDetails problemDetails = objectResult.Value as ValidationProblemDetails;
                Assert.Equal(3, problemDetails.Errors.Count);
                Assert.Equal(3, problemDetails.Errors["property1"].Length);
                Assert.Single(problemDetails.Errors["property2"]);
                Assert.Single(problemDetails.Errors["property3"]);
            }
        }

        public class OnUncaughtException : ExceptionFilterUnitTests
        {
            [Fact]
            public void When_HandlingUncaughtException_Expect_NullResult()
            {
                // Arrange
                ExceptionContext exceptionContext = new ExceptionContext(this.actionContext, new List<IFilterMetadata>())
                {
                    Exception = new Exception(),
                };

                ExceptionFilter exceptionFilter = new ExceptionFilter(null);

                // Act
                exceptionFilter.OnException(exceptionContext);

                // Assert
                Assert.Null(exceptionContext.Result);
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
