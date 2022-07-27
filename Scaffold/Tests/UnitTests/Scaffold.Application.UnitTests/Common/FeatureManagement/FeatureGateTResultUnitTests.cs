namespace Scaffold.Application.UnitTests.Common.FeatureManagement;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Scaffold.Application.Common.FeatureManagement;
using Scaffold.Application.Common.Instrumentation;
using Xunit;

[Collection(TestCollection.Activity)]
public class FeatureGateTResultUnitTests
{
    public class Invoke
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

    public class WhenOpened
    {
        [Fact]
        public void When_FeatureGateInvokedWithWhenOpenAction_Expect_NewFeatureGateWithNewAction()
        {
            // Arrange
            bool isOpened = true;

            FeatureGate<string> featureGate = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => "Feature gate was opened!",
                whenClosed: () => "Feature gate was closed.");

            // Act
            FeatureGate<string> newFeatureGate = featureGate.WhenOpened(() => "Updated Action.");
            string whenOpened = newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Updated Action.", whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenOpenFuncTask_Expect_NewFeatureGateWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            FeatureGate<string> featureGate = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => "Feature gate was opened!",
                whenClosed: () => "Feature gate was closed.");

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenOpened(() => Task.Run(() => "Updated Function."));
            string whenOpened = await newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = await newFeatureGate.Invoke();

            // Assert
            Assert.Equal("Updated Function.", whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenOpenFuncTaskAndWhenClosedNull_Expect_NewFeatureGateWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            FeatureGate<string> featureGate = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => "Feature gate was opened!",
                whenClosed: null);

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenOpened(() => Task.Run(() => "Updated Function."));
            string whenOpened = await newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = await newFeatureGate.Invoke();

            // Assert
            Assert.Equal("Updated Function.", whenOpened);
            Assert.Null(whenClosed);
        }
    }

    public class WhenClosed
    {
        [Fact]
        public void When_FeatureGateInvokedWithWhenClosedAction_Expect_NewFeatureGateWithNewAction()
        {
            // Arrange
            bool isOpened = true;

            FeatureGate<string> featureGate = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => "Feature gate was opened!",
                whenClosed: () => "Feature gate was closed.");

            // Act
            FeatureGate<string> newFeatureGate = featureGate.WhenClosed(() => "Updated Action.");
            string whenOpened = newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Equal("Updated Action.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenClosedFuncTask_Expect_NewFeatureGateWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            FeatureGate<string> featureGate = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => "Feature gate was opened!",
                whenClosed: () => "Feature gate was closed.");

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenClosed(() => Task.Run(() => "Updated Function."));
            string whenOpened = await newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = await newFeatureGate.Invoke();

            // Assert
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Equal("Updated Function.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenClosedFuncTaskAndWhenOpenedNull_Expect_NewFeatureGateWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            FeatureGate<string> featureGate = new FeatureGate<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: null,
                whenClosed: () => "Feature gate was closed.");

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenClosed(() => Task.Run(() => "Updated Function."));
            string whenOpened = await newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = await newFeatureGate.Invoke();

            // Assert
            Assert.Null(whenOpened);
            Assert.Equal("Updated Function.", whenClosed);
        }
    }
}
