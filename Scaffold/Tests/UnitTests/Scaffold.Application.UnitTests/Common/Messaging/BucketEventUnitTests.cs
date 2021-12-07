namespace Scaffold.Application.UnitTests.Common.Messaging;

using System;
using System.Diagnostics;
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

        TestBucketEvent result = new TestBucketEvent(null, string.Empty, string.Empty);

        // Assert
        Assert.Equal(activity.TraceId.ToHexString(), result.TraceId);
    }

    [Fact]
    public void When_InstantiatingBucketEvent_Expect_NullTraceId()
    {
        // Act
        TestBucketEvent result = new TestBucketEvent(null, string.Empty, string.Empty);

        // Assert
        Assert.Null(result.TraceId);
    }

    private record TestBucketEvent(Type Source, string Type, string Description)
        : BucketEvent(Source, Type, Description);
}
