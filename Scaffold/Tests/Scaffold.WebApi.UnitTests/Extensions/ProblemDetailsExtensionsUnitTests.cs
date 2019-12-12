namespace Scaffold.WebApi.UnitTests.Extensions
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Scaffold.WebApi.Extensions;
    using Xunit;

    public class ProblemDetailsExtensionsUnitTests
    {
        public class AddTraceId
        {
            [Fact]
            public void When_AddingTraceId_Expect_TraceIdAdded()
            {
                // Arrange
                ProblemDetails details = new ProblemDetails();

                // Act
                Activity activity = new Activity("Unit Test")
                    .SetIdFormat(ActivityIdFormat.W3C)
                    .Start();

                details.AddW3cTraceId();

                activity.Stop();

                // Assert
                Assert.Equal(activity.TraceId.ToString(), details.Extensions["traceId"]);
            }

            [Fact]
            public void When_AddingTraceIdWithNullActivity_Expect_NoTraceIdAdded()
            {
                // Arrange
                Activity.Current = null;

                ProblemDetails details = new ProblemDetails();

                // Act
                details.AddW3cTraceId();

                // Assert
                Assert.Empty(details.Extensions);
            }

            [Fact]
            public void When_AddingTraceIdWithNonW3cFormatId_Expect_NoTraceIdAdded()
            {
                // Arrange
                ProblemDetails details = new ProblemDetails();

                // Act
                Activity activity = new Activity("Unit Test")
                    .SetIdFormat(ActivityIdFormat.Hierarchical)
                    .Start();

                details.AddW3cTraceId();

                activity.Stop();

                // Assert
                Assert.Empty(details.Extensions);
            }
        }
    }
}
