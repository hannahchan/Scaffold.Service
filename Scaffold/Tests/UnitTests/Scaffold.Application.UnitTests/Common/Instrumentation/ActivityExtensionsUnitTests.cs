namespace Scaffold.Application.UnitTests.Common.Instrumentation
{
    using System;
    using System.Diagnostics;
    using Scaffold.Application.Common.Instrumentation;
    using Xunit;

    public static class ActivityExtensionsUnitTests
    {
        public class InvokeIfNotNullAndIsAllDataRequested
        {
            [Fact]
            public void When_ActivityIsNull_Expect_()
            {
                // Arrange
                Activity activity = null;

                // Act
                Exception exception = Record.Exception(() => activity.InvokeIfNotNullAndIsAllDataRequested(activity =>
                    activity.SetTag("Key", "Value")));

                // Assert
                Assert.Null(activity);
                Assert.Null(exception);
            }

            [Fact]
            public void When_ActivityIsAllDataRequestIsFalse_Expect_()
            {
                // Arrange
                Activity activity = new Activity(Guid.NewGuid().ToString())
                {
                    IsAllDataRequested = false,
                };

                // Act
                Exception exception = Record.Exception(() => activity.InvokeIfNotNullAndIsAllDataRequested(activity =>
                    activity.SetTag("Key", "Value")));

                // Assert
                Assert.Null(exception);
                Assert.Empty(activity.Tags);
            }

            [Fact]
            public void When_ActivityIsNotNullAndIsAllDataRequestedIsTrue_Expect_()
            {
                // Arrange
                Activity activity = new Activity(Guid.NewGuid().ToString())
                {
                    IsAllDataRequested = true,
                };

                // Act
                Exception exception = Record.Exception(() => activity.InvokeIfNotNullAndIsAllDataRequested(activity =>
                    activity.SetTag("Key", "Value")));

                // Assert
                Assert.Null(exception);
                Assert.Collection(activity.Tags, tag =>
                {
                    Assert.Equal("Key", tag.Key);
                    Assert.Equal("Value", tag.Value);
                });
            }
        }
    }
}
