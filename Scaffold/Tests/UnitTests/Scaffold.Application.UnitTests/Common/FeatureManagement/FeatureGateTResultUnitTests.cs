namespace Scaffold.Application.UnitTests.Common.FeatureManagement;

using System;
using System.Diagnostics;
using Scaffold.Application.Common.FeatureManagement;
using Scaffold.Application.Common.Instrumentation;
using Xunit;

[Collection(TestCollection.Activity)]
public class FeatureGateTResultUnitTests
{
    [Theory]
    [InlineData(true, "Feature gate was opened!")]
    [InlineData(false, "Feature gate was closed.")]
    public void When_FeatureGateInvokedWithFunction_Expect_Function(bool isOpened, string expected)
    {
        // Arrange
        string result = string.Empty;

        // Act
        result = new FeatureGate<string>(
            featureGateKey: "myFeatureGateKey",
            controlledBy: () => isOpened,
            whenOpened: () => "Feature gate was opened!",
            whenClosed: () => "Feature gate was closed.")
            .Invoke();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void When_FeatureGateInvokedWithNullFunction_Expect_NoException(bool isOpened)
    {
        // Arrange
        string result = string.Empty;
        Exception exception;

        // Act
        exception = Record.Exception(() => result = new FeatureGate<string>(
            featureGateKey: "myFeatureGateKey",
            controlledBy: () => isOpened,
            whenOpened: null,
            whenClosed: null)
            .Invoke());

        // Assert
        Assert.Null(result);
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(true, "Opened gate threw an exception.")]
    [InlineData(false, "Closed gate threw an exception.")]
    public void When_FeatureGateInvokedWithFunctionThrowingException_Expect_Exception(bool isOpened, string expected)
    {
        // Arrange
        Exception result;

        // Act
        result = Record.Exception(() => new FeatureGate<string>(
            featureGateKey: "myFeatureGateKey",
            controlledBy: () => isOpened,
            whenOpened: () => throw new Exception("Opened gate threw an exception."),
            whenClosed: () => throw new Exception("Closed gate threw an exception."))
            .Invoke());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result.Message);
    }

    [Fact]
    public void When_FeatureGateInvokedWithActivity_Expect_NoException()
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
        Exception gateOpenedResult = Record.Exception(() => new FeatureGate<string>(
            featureGateKey: "myFeatureGateKey",
            metricType: MetricType.Histogram,
            controlledBy: () => true,
            whenOpened: null,
            whenClosed: null)
            .Invoke());

        Exception gateClosedResult = Record.Exception(() => new FeatureGate<string>(
            featureGateKey: "myFeatureGateKey",
            metricType: MetricType.Histogram,
            controlledBy: () => false,
            whenOpened: null,
            whenClosed: null)
            .Invoke());

        // Assert
        Assert.Null(gateOpenedResult);
        Assert.Null(gateClosedResult);
    }

    [Fact]
    public void When_FeatureGateInvokedWithActivity_Expect_Exception()
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
        Exception gateOpenedResult = Record.Exception(() => new FeatureGate<string>(
            featureGateKey: "myFeatureGateKey",
            metricType: MetricType.Histogram,
            controlledBy: () => true,
            whenOpened: () => throw new Exception("Opened gate threw an exception."),
            whenClosed: () => throw new Exception("Closed gate threw an exception."))
            .Invoke());

        Exception gateClosedResult = Record.Exception(() => new FeatureGate<string>(
            featureGateKey: "myFeatureGateKey",
            metricType: MetricType.Histogram,
            controlledBy: () => false,
            whenOpened: () => throw new Exception("Opened gate threw an exception."),
            whenClosed: () => throw new Exception("Closed gate threw an exception."))
            .Invoke());

        // Assert
        Assert.NotNull(gateOpenedResult);
        Assert.Equal("Opened gate threw an exception.", gateOpenedResult.Message);

        Assert.NotNull(gateClosedResult);
        Assert.Equal("Closed gate threw an exception.", gateClosedResult.Message);
    }

    [Theory]
    [InlineData(MetricType.Counter)]
    [InlineData(MetricType.Histogram)]
    internal void When_FeatureGateInvokedWithMetricType_Expect_NoException(MetricType metricType)
    {
        // Arrange
        Exception gateOpenedResult;
        Exception gateClosedResult;

        // Act
        gateOpenedResult = Record.Exception(() => new FeatureGate<string>(
            featureGateKey: "myFeatureGateKey",
            metricType: metricType,
            controlledBy: () => true,
            whenOpened: null,
            whenClosed: null)
            .Invoke());

        gateClosedResult = Record.Exception(() => new FeatureGate<string>(
            featureGateKey: "myFeatureGateKey",
            metricType: metricType,
            controlledBy: () => false,
            whenOpened: null,
            whenClosed: null)
            .Invoke());

        // Assert
        Assert.Null(gateOpenedResult);
        Assert.Null(gateClosedResult);
    }

    [Theory]
    [InlineData(MetricType.Counter)]
    [InlineData(MetricType.Histogram)]
    internal void When_FeatureGateInvokedWithMetricType_Expect_Exception(MetricType metricType)
    {
        // Arrange
        Exception gateOpenedResult;
        Exception gateClosedResult;

        // Act
        gateOpenedResult = Record.Exception(() => new FeatureGate<string>(
            featureGateKey: "myFeatureGateKey",
            metricType: metricType,
            controlledBy: () => true,
            whenOpened: () => throw new Exception("Opened gate threw an exception."),
            whenClosed: () => throw new Exception("Closed gate threw an exception."))
            .Invoke());

        gateClosedResult = Record.Exception(() => new FeatureGate<string>(
            featureGateKey: "myFeatureGateKey",
            metricType: metricType,
            controlledBy: () => false,
            whenOpened: () => throw new Exception("Opened gate threw an exception."),
            whenClosed: () => throw new Exception("Closed gate threw an exception."))
            .Invoke());

        // Assert
        Assert.NotNull(gateOpenedResult);
        Assert.Equal("Opened gate threw an exception.", gateOpenedResult.Message);

        Assert.NotNull(gateClosedResult);
        Assert.Equal("Closed gate threw an exception.", gateClosedResult.Message);
    }
}
