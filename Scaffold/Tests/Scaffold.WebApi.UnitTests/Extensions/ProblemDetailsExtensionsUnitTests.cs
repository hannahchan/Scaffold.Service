namespace Scaffold.WebApi.UnitTests.Extensions
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using OpenTracing.Mock;
    using Scaffold.WebApi.Extensions;
    using Xunit;

    public class ProblemDetailsExtensionsUnitTests
    {
        public class AddOpenTracingTraceId
        {
            [Fact]
            public void When_AddingTraceId_Expect_TraceIdAdded()
            {
                // Arrange
                MockTracer mockTracer = new MockTracer();
                ProblemDetails details = new ProblemDetails();

                // Act
                using (mockTracer.BuildSpan("Unit Test").StartActive())
                {
                    details.AddOpenTracingTraceId(mockTracer);
                }

                // Assert
                MockSpan mockSpan = Assert.Single(mockTracer.FinishedSpans());
                Assert.Equal(mockSpan.Context.TraceId, details.Extensions["traceId"]);
            }

            [Fact]
            public void When_AddingTraceIdWithNullProblemDetails_Expect_ArgumentNullException()
            {
                // Arrange
                ProblemDetails? details = null;

                // Act
                Exception exception = Record.Exception(() => details!.AddOpenTracingTraceId(new MockTracer()));

                // Assert
                Assert.IsType<ArgumentNullException>(exception);
            }

            [Fact]
            public void When_AddingTraceIdWithNullTracer_Expect_ArgumentNullException()
            {
                // Arrange
                ProblemDetails details = new ProblemDetails();

                // Act
                Exception exception = Record.Exception(() => details.AddOpenTracingTraceId(null!));

                // Assert
                Assert.IsType<ArgumentNullException>(exception);
            }

            [Fact]
            public void When_AddingTraceIdWithNullActiveSpan_Expect_NoTraceIdAdded()
            {
                // Arrange
                MockTracer mockTracer = new MockTracer();
                ProblemDetails details = new ProblemDetails();

                // Act
                details.AddOpenTracingTraceId(mockTracer);

                // Assert
                Assert.Empty(mockTracer.FinishedSpans());
                Assert.Empty(details.Extensions);
            }
        }
    }
}
