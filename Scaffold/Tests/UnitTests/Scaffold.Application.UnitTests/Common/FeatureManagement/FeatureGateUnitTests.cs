namespace Scaffold.Application.UnitTests.Common.FeatureManagement;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Scaffold.Application.Common.FeatureManagement;
using Scaffold.Application.Common.Instrumentation;
using Xunit;

[Collection(TestCollection.Activity)]
public class FeatureGateUnitTests
{
    public class WithKey
    {
        [Fact]
        public void When_InvokingStaticMethodWithKey_Expect_FeatureGateBuilder()
        {
            // Arrange
            string featureGateKey = Guid.NewGuid().ToString();

            // Act
            FeatureGateBuilder builder = FeatureGate.WithKey(featureGateKey);

            // Assert
            Assert.Equal(featureGateKey, builder.Key);
        }
    }

    public class Invoke
    {
        [Theory]
        [InlineData(true, "Feature gate was opened!")]
        [InlineData(false, "Feature gate was closed.")]
        public void When_FeatureGateInvokedWithAction_Expect_Action(bool isOpened, string expected)
        {
            // Arrange
            string result = string.Empty;

            // Act
            new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => result = "Feature gate was opened!",
                whenClosed: () => result = "Feature gate was closed.")
                .Invoke();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void When_FeatureGateInvokedWithNullAction_Expect_NoException(bool isOpened)
        {
            // Arrange
            Exception result;

            // Act
            result = Record.Exception(() => new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: null,
                whenClosed: null)
                .Invoke());

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(true, "Opened gate threw an exception.")]
        [InlineData(false, "Closed gate threw an exception.")]
        public void When_FeatureGateInvokedWithActionThrowingException_Expect_Exception(bool isOpened, string expected)
        {
            // Arrange
            Exception result;

            // Act
            result = Record.Exception(() => new FeatureGate(
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
            Exception gateOpenedResult = Record.Exception(() => new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                metricType: MetricType.Histogram,
                controlledBy: () => true,
                whenOpened: null,
                whenClosed: null)
                .Invoke());

            Exception gateClosedResult = Record.Exception(() => new FeatureGate(
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
            Exception gateOpenedResult = Record.Exception(() => new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                metricType: MetricType.Histogram,
                controlledBy: () => true,
                whenOpened: () => throw new Exception("Opened gate threw an exception."),
                whenClosed: () => throw new Exception("Closed gate threw an exception."))
                .Invoke());

            Exception gateClosedResult = Record.Exception(() => new FeatureGate(
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
            gateOpenedResult = Record.Exception(() => new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                metricType: metricType,
                controlledBy: () => true,
                whenOpened: null,
                whenClosed: null)
                .Invoke());

            gateClosedResult = Record.Exception(() => new FeatureGate(
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
            gateOpenedResult = Record.Exception(() => new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                metricType: metricType,
                controlledBy: () => true,
                whenOpened: () => throw new Exception("Opened gate threw an exception."),
                whenClosed: () => throw new Exception("Closed gate threw an exception."))
                .Invoke());

            gateClosedResult = Record.Exception(() => new FeatureGate(
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

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGate featureGate = new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => whenOpened = "Feature gate was opened!",
                whenClosed: () => whenClosed = "Feature gate was closed.");

            // Act
            FeatureGate newFeatureGate = featureGate.WhenOpened(() => whenOpened = "Updated Action.");
            newFeatureGate.Invoke();

            isOpened = false;
            newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Updated Action.", whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenOpenFuncTask_Expect_NewGateAsyncWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGate featureGate = new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => whenOpened = "Feature gate was opened!",
                whenClosed: () => whenClosed = "Feature gate was closed.");

            // Act
            FeatureGateAsync newFeatureGate = featureGate.WhenOpened(() => Task.Run(() => whenOpened = "Updated Function."));
            await newFeatureGate.Invoke();

            isOpened = false;
            await newFeatureGate.Invoke();

            // Assert
            Assert.Equal("Updated Function.", whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenOpenFuncTaskAndWhenClosedNull_Expect_NewGateAsyncWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGate featureGate = new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => whenOpened = "Feature gate was opened!",
                whenClosed: null);

            // Act
            FeatureGateAsync newFeatureGate = featureGate.WhenOpened(() => Task.Run(() => whenOpened = "Updated Function."));
            await newFeatureGate.Invoke();

            isOpened = false;
            await newFeatureGate.Invoke();

            // Assert
            Assert.Equal("Updated Function.", whenOpened);
            Assert.Equal(string.Empty, whenClosed);
        }
    }

    public class WhenClosed
    {
        [Fact]
        public void When_FeatureGateInvokedWithWhenClosedAction_Expect_NewFeatureGateWithNewAction()
        {
            // Arrange
            bool isOpened = true;

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGate featureGate = new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => whenOpened = "Feature gate was opened!",
                whenClosed: () => whenClosed = "Feature gate was closed.");

            // Act
            FeatureGate newFeatureGate = featureGate.WhenClosed(() => whenClosed = "Updated Action.");
            newFeatureGate.Invoke();

            isOpened = false;
            newFeatureGate.Invoke();

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

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGate featureGate = new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: () => whenOpened = "Feature gate was opened!",
                whenClosed: () => whenClosed = "Feature gate was closed.");

            // Act
            FeatureGateAsync newFeatureGate = featureGate.WhenClosed(() => Task.Run(() => whenClosed = "Updated Function."));
            await newFeatureGate.Invoke();

            isOpened = false;
            await newFeatureGate.Invoke();

            // Assert
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Equal("Updated Function.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenClosedFuncTaskAndWhenOpenNull_Expect_NewFeatureGateWithFuncTask()
        {
            // Arrange
            bool isOpened = true;

            string whenOpened = string.Empty;
            string whenClosed = string.Empty;

            FeatureGate featureGate = new FeatureGate(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => isOpened,
                whenOpened: null,
                whenClosed: () => whenClosed = "Feature gate was closed.");

            // Act
            FeatureGateAsync newFeatureGate = featureGate.WhenClosed(() => Task.Run(() => whenClosed = "Updated Function."));
            await newFeatureGate.Invoke();

            isOpened = false;
            await newFeatureGate.Invoke();

            // Assert
            Assert.Equal(string.Empty, whenOpened);
            Assert.Equal("Updated Function.", whenClosed);
        }
    }
}
