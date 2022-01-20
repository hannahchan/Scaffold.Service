namespace Scaffold.Application.UnitTests.Common.Messaging;

using System.Diagnostics;
using Scaffold.Application.Common.Instrumentation;
using Scaffold.Application.Common.Messaging;
using Xunit;

[Collection(TestCollection.Activity)]
public class BucketEventUnitTests
{
    [Fact]
    public void When_InstantiatingBucketEvent_Expect_TraceId()
    {
        // Arrange
        using Activity activity = new Activity(nameof(activity));

        // Act
        activity.Start();

        TestBucketEvent result = new TestBucketEvent(string.Empty, string.Empty);

        // Assert
        Assert.Equal(activity.GetTraceId(), result.TraceId);
    }

    [Fact]
    public void When_InstantiatingBucketEvent_Expect_NullTraceId()
    {
        // Act
        TestBucketEvent result = new TestBucketEvent(string.Empty, string.Empty);

        // Assert
        Assert.Null(result.TraceId);
    }

    private record TestBucketEvent(string Type, string Description)
        : BucketEvent(Type, Description);
}
