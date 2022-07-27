namespace Scaffold.Application.UnitTests.Common.FeatureManagement;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Scaffold.Application.Common.FeatureManagement;
using Scaffold.Application.Common.Instrumentation;
using Xunit;

[Collection(TestCollection.Activity)]
public class FeatureGateAsyncTResultUnitTests
{
    public class Invoke
    {
        [Theory]
        [InlineData(true, "Feature gate was opened!")]
        [InlineData(false, "Feature gate was closed.")]
        public async Task When_FeatureGateInvokedWithFunction_Expect_Function(bool isOpened, string expected)
        {
            // Arrange
            string result = string.Empty;

            // Act
            result = await new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: async () => await Task.FromResult(isOpened),
                whenOpened: async () => await Task.FromResult("Feature gate was opened!"),
                whenClosed: async () => await Task.FromResult("Feature gate was closed."))
                .Invoke();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task When_FeatureGateInvokedWithNullFunction_Expect_NoException(bool isOpened)
        {
            // Arrange
            string result = string.Empty;
            Exception exception;

            // Act
            exception = await Record.ExceptionAsync(async () => result = await new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: async () => await Task.FromResult(isOpened),
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
        public async Task When_FeatureGateInvokedWithFunctionThrowingException_Expect_Exception(bool isOpened, string expected)
        {
            // Arrange
            Exception result;

            // Act
            result = await Record.ExceptionAsync(() => new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: async () => await Task.FromResult(isOpened),
                whenOpened: () => throw new Exception("Opened gate threw an exception."),
                whenClosed: () => throw new Exception("Closed gate threw an exception."))
                .Invoke());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, result.Message);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithActivity_Expect_NoException()
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
            Exception gateOpenedResult = await Record.ExceptionAsync(() => new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                metricType: MetricType.Histogram,
                controlledBy: async () => await Task.FromResult(true),
                whenOpened: null,
                whenClosed: null)
                .Invoke());

            Exception gateClosedResult = await Record.ExceptionAsync(() => new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                metricType: MetricType.Histogram,
                controlledBy: async () => await Task.FromResult(false),
                whenOpened: null,
                whenClosed: null)
                .Invoke());

            // Assert
            Assert.Null(gateOpenedResult);
            Assert.Null(gateClosedResult);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithActivity_Expect_Exception()
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
            Exception gateOpenedResult = await Record.ExceptionAsync(() => new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                metricType: MetricType.Histogram,
                controlledBy: async () => await Task.FromResult(true),
                whenOpened: () => throw new Exception("Opened gate threw an exception."),
                whenClosed: () => throw new Exception("Closed gate threw an exception."))
                .Invoke());

            Exception gateClosedResult = await Record.ExceptionAsync(() => new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                metricType: MetricType.Histogram,
                controlledBy: async () => await Task.FromResult(false),
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
        internal async Task When_FeatureGateInvokedWithMetricType_Expect_NoException(MetricType metricType)
        {
            // Arrange
            Exception gateOpenedResult;
            Exception gateClosedResult;

            // Act
            gateOpenedResult = await Record.ExceptionAsync(() => new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                metricType: metricType,
                controlledBy: async () => await Task.FromResult(true),
                whenOpened: null,
                whenClosed: null)
                .Invoke());

            gateClosedResult = await Record.ExceptionAsync(() => new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                metricType: metricType,
                controlledBy: async () => await Task.FromResult(false),
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
        internal async Task When_FeatureGateInvokedWithMetricType_Expect_Exception(MetricType metricType)
        {
            // Arrange
            Exception gateOpenedResult;
            Exception gateClosedResult;

            // and Act
            gateOpenedResult = await Record.ExceptionAsync(() => new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                metricType: metricType,
                controlledBy: async () => await Task.FromResult(true),
                whenOpened: () => throw new Exception("Opened gate threw an exception."),
                whenClosed: () => throw new Exception("Closed gate threw an exception."))
                .Invoke());

            gateClosedResult = await Record.ExceptionAsync(() => new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                metricType: metricType,
                controlledBy: async () => await Task.FromResult(false),
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
        public async Task When_FeatureGateInvokedWithWhenOpenFuncTask_Expect_NewFeatureGateWithNewFuncTask()
        {
            // Arrange
            bool isOpened = true;

            FeatureGateAsync<string> featureGate = new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => "Feature gate was closed."));

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
        public async Task When_FeatureGateInvokedWithWhenOpenFunc_Expect_NewFeatureGateWithNewFunc()
        {
            // Arrange
            bool isOpened = true;

            FeatureGateAsync<string> featureGate = new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => "Feature gate was closed."));

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenOpened(() => "Updated Action.");
            string whenOpened = await newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = await newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Updated Action.", whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenOpenNullFunc_Expect_NewFeatureGateWithNewFunc()
        {
            // Arrange
            bool isOpened = true;

            FeatureGateAsync<string> featureGate = new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => "Feature gate was closed."));

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenOpened(null as Func<string>);
            string whenOpened = await newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = await newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Null(whenOpened);
            Assert.Equal("Feature gate was closed.", whenClosed);
        }
    }

    public class WhenClosed
    {
        [Fact]
        public async Task When_FeatureGateInvokedWithWhenClosedFuncTask_Expect_NewFeatureGateWithNewFuncTask()
        {
            // Arrange
            bool isOpened = true;

            FeatureGateAsync<string> featureGate = new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => "Feature gate was closed."));

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
        public async Task When_FeatureGateInvokedWithWhenClosedFunc_Expect_NewFeatureGateWithNewFunc()
        {
            // Arrange
            bool isOpened = true;

            FeatureGateAsync<string> featureGate = new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => "Feature gate was closed."));

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenClosed(() => "Updated Action.");
            string whenOpened = await newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = await newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Equal("Updated Action.", whenClosed);
        }

        [Fact]
        public async Task When_FeatureGateInvokedWithWhenClosedNullFunc_Expect_NewFeatureGateWithNewFunc()
        {
            // Arrange
            bool isOpened = true;

            FeatureGateAsync<string> featureGate = new FeatureGateAsync<string>(
                featureGateKey: "myFeatureGateKey",
                controlledBy: () => Task.FromResult(isOpened),
                whenOpened: () => Task.Run(() => "Feature gate was opened!"),
                whenClosed: () => Task.Run(() => "Feature gate was closed."));

            // Act
            FeatureGateAsync<string> newFeatureGate = featureGate.WhenClosed(null as Func<string>);
            string whenOpened = await newFeatureGate.Invoke();

            isOpened = false;
            string whenClosed = await newFeatureGate.Invoke();

            // Assert
            Assert.NotEqual(featureGate, newFeatureGate);
            Assert.Equal("Feature gate was opened!", whenOpened);
            Assert.Null(whenClosed);
        }
    }
}
