namespace Scaffold.Application.UnitTests.Common.Instrumentation;

using System;
using System.Diagnostics;
using Scaffold.Application.Common.Instrumentation;
using Xunit;

public static class ActivityExtensionsUnitTests
{
    [Collection(TestCollection.Activity)]
    public class GetSpanId
    {
        [Fact]
        public void When_GettingSpanIdWithHierarchicalFormat_Expect_ActivityId()
        {
            // Arrange
            Activity.DefaultIdFormat = ActivityIdFormat.Hierarchical;

            using Activity activity = new Activity(nameof(activity));

            // Act
            activity.Start();

            string result = activity.GetSpanId();

            // Assert
            Assert.Equal(activity.Id, result);
        }

        [Fact]
        public void When_GettingSpanIdWithW3CFormat_Expect_ActivitySpanId()
        {
            // Arrange
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            using Activity activity = new Activity(nameof(activity));

            // Act
            activity.Start();

            string result = activity.GetSpanId();

            // Assert
            Assert.Equal(activity.SpanId.ToHexString(), result);
        }

        [Fact]
        public void When_GettingSpanIdWithNotStartedActivity_Expect_Null()
        {
            // Arrange
            using Activity activity = new Activity(nameof(activity));

            // Act
            string result = activity.GetSpanId();

            // Assert
            Assert.Null(result);
        }
    }

    [Collection(TestCollection.Activity)]
    public class GetTraceId
    {
        [Fact]
        public void When_GettingTraceIdWithHierarchicalFormat_Expect_ActivityRootId()
        {
            // Arrange
            Activity.DefaultIdFormat = ActivityIdFormat.Hierarchical;

            using Activity activity = new Activity(nameof(activity));

            // Act
            activity.Start();

            string result = activity.GetTraceId();

            // Assert
            Assert.Equal(activity.RootId, result);
        }

        [Fact]
        public void When_GettingTraceIdWithW3CFormat_Expect_ActivityTraceId()
        {
            // Arrange
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            using Activity activity = new Activity(nameof(activity));

            // Act
            activity.Start();

            string result = activity.GetTraceId();

            // Assert
            Assert.Equal(activity.TraceId.ToHexString(), result);
        }

        [Fact]
        public void When_GettingTraceIdWithNotStartedActivity_Expect_Null()
        {
            // Arrange
            using Activity activity = new Activity(nameof(activity));

            // Act
            string result = activity.GetTraceId();

            // Assert
            Assert.Null(result);
        }
    }

    [Collection(TestCollection.Activity)]
    public class GetParentId
    {
        [Fact]
        public void When_GettingParentIdWithHierarchicalFormat_Expect_ActivityParentId()
        {
            // Arrange
            Activity.DefaultIdFormat = ActivityIdFormat.Hierarchical;

            using Activity parentActivity = new Activity(nameof(parentActivity));
            using Activity childActivity = new Activity(nameof(childActivity));

            // Act
            parentActivity.Start();
            childActivity.Start();

            string result = childActivity.GetParentId();

            // Assert
            Assert.Equal(parentActivity.Id, result);
            Assert.Equal(childActivity.ParentId, result);
        }

        [Fact]
        public void When_GettingParentIdWithW3CFormat_Expect_ActivityParentSpanId()
        {
            // Arrange
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            using Activity parentActivity = new Activity(nameof(parentActivity));
            using Activity childActivity = new Activity(nameof(childActivity));

            // Act
            parentActivity.Start();
            childActivity.Start();

            string result = childActivity.GetParentId();

            // Assert
            Assert.Equal(parentActivity.SpanId.ToHexString(), result);
            Assert.Equal(childActivity.ParentSpanId.ToHexString(), result);
        }

        [Fact]
        public void When_GettingParentIdWithNotStartedActivity_Expect_Null()
        {
            // Arrange
            using Activity activity = new Activity(nameof(activity));

            // Act
            string result = activity.GetParentId();

            // Assert
            Assert.Null(result);
        }
    }

    [Collection(TestCollection.Activity)]
    public class InvokeIfNotNullAndIsAllDataRequested
    {
        [Fact]
        public void When_ActivityIsNull_Expect_NoException()
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
        public void When_ActivityIsAllDataRequestIsFalse_Expect_NoException()
        {
            // Arrange
            using Activity activity = new Activity(Guid.NewGuid().ToString())
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
        public void When_ActivityIsNotNullAndIsAllDataRequestedIsTrue_Expect_TagSet()
        {
            // Arrange
            using Activity activity = new Activity(Guid.NewGuid().ToString())
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
