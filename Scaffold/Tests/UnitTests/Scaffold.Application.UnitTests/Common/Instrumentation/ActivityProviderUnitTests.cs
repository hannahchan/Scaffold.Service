namespace Scaffold.Application.UnitTests.Common.Instrumentation;

using System.Diagnostics;
using Scaffold.Application.Common.Instrumentation;
using Xunit;

[Collection(TestCollection.Activity)]
public class ActivityProviderUnitTests
{
    [Fact]
    public void When_StartingActivity_Expect_Activity()
    {
        // Arrange
        using ActivityListener listener = new ActivityListener
        {
            ShouldListenTo = source => true,
            SampleUsingParentId = (ref ActivityCreationOptions<string> options) => ActivitySamplingResult.AllData,
            Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
        };

        ActivityProvider.AddActivityListener(listener);

        // Act
        using Activity activity = ActivityProvider.StartActivity(nameof(this.When_StartingActivity_Expect_Activity));

        // Assert
        Assert.True(ActivityProvider.HasListeners());
        Assert.NotNull(activity);
    }

    [Fact]
    public void When_StartingActivity_Expect_NullActivity()
    {
        // Arrange and Act
        using Activity activity = ActivityProvider.StartActivity(nameof(this.When_StartingActivity_Expect_NullActivity));

        // Assert
        Assert.False(ActivityProvider.HasListeners());
        Assert.Null(activity);
    }
}
