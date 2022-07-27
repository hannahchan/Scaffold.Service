namespace Scaffold.Application.UnitTests.Common.FeatureManagement;

using System;
using System.Threading.Tasks;
using Scaffold.Application.Common.FeatureManagement;
using Xunit;

public class FeatureGateBuilderUnitTests
{
    public class BooleanFunction
    {
        [Fact]
        public void When_BuildingFeatureGateWithAction_Expect_FeatureGate()
        {
            // Arrange
            string featureGateKey = Guid.NewGuid().ToString();

            // Act
            FeatureGate featureGate = new FeatureGateBuilder(featureGateKey)
                .WithHistogram()
                .ControlledBy(Test.BooleanFunction)
                .WhenOpened(Test.Action)
                .WhenClosed(Test.Action);

            // Assert
            Assert.Equal(featureGateKey, featureGate.Key);
            Assert.Equal(MetricType.Histogram, featureGate.MetricType);
        }

        [Fact]
        public void When_BuildingFeatureGateWithFunction_Expect_FeatureGateWithReturnValue()
        {
            // Arrange
            string featureGateKey = Guid.NewGuid().ToString();

            // Act
            FeatureGate<string> featureGate = new FeatureGateBuilder(featureGateKey)
                .ControlledBy(Test.BooleanFunction)
                .WhenOpened(Test.Function<string>)
                .WhenClosed(Test.Function<string>);

            // Assert
            Assert.Equal(featureGateKey, featureGate.Key);
            Assert.Equal(MetricType.Counter, featureGate.MetricType);
        }

        [Fact]
        public void When_BuildingFeatureGateWithActionAsync_Expect_FeatureGateAsync()
        {
            // Arrange
            string featureGateKey = Guid.NewGuid().ToString();

            // Act
            FeatureGateAsync featureGate = new FeatureGateBuilder(featureGateKey)
                .ControlledBy(Test.BooleanFunction)
                .WhenOpened(Test.ActionAsync)
                .WhenClosed(Test.ActionAsync);

            // Assert
            Assert.Equal(featureGateKey, featureGate.Key);
            Assert.Equal(MetricType.Counter, featureGate.MetricType);
        }

        [Fact]
        public void When_BuildingFeatureGateWithFunctionAsync_Expect_FeatureGateAsyncWithReturnValue()
        {
            // Arrange
            string featureGateKey = Guid.NewGuid().ToString();

            // Act
            FeatureGateAsync<string> featureGate = new FeatureGateBuilder(featureGateKey)
                .ControlledBy(Test.BooleanFunction)
                .WhenOpened(Test.FunctionAsync<string>)
                .WhenClosed(Test.FunctionAsync<string>);

            // Assert
            Assert.Equal(featureGateKey, featureGate.Key);
            Assert.Equal(MetricType.Counter, featureGate.MetricType);
        }
    }

    public class BooleanFunctionAsync
    {
        [Fact]
        public void When_BuildingFeatureGateWithAction_Expect_FeatureGateAsync()
        {
            // Arrange
            string featureGateKey = Guid.NewGuid().ToString();

            // Act
            FeatureGateAsync featureGate = new FeatureGateBuilder(featureGateKey)
                .WithHistogram()
                .ControlledBy(Test.BooleanFunctionAsync)
                .WhenOpened(Test.Action)
                .WhenClosed(Test.Action);

            // Assert
            Assert.Equal(featureGateKey, featureGate.Key);
            Assert.Equal(MetricType.Histogram, featureGate.MetricType);
        }

        [Fact]
        public void When_BuildingFeatureGateWithFunction_Expect_FeatureGateAsyncWithReturnValue()
        {
            // Arrange
            string featureGateKey = Guid.NewGuid().ToString();

            // Act
            FeatureGateAsync<string> featureGate = new FeatureGateBuilder(featureGateKey)
                .ControlledBy(Test.BooleanFunctionAsync)
                .WhenOpened(Test.Function<string>)
                .WhenClosed(Test.Function<string>);

            // Assert
            Assert.Equal(featureGateKey, featureGate.Key);
            Assert.Equal(MetricType.Counter, featureGate.MetricType);
        }

        [Fact]
        public void When_BuildingFeatureGateWithActionAsync_Expect_FeatureGateAsync()
        {
            // Arrange
            string featureGateKey = Guid.NewGuid().ToString();

            // Act
            FeatureGateAsync featureGate = new FeatureGateBuilder(featureGateKey)
                .ControlledBy(Test.BooleanFunctionAsync)
                .WhenOpened(Test.ActionAsync)
                .WhenClosed(Test.ActionAsync);

            // Assert
            Assert.Equal(featureGateKey, featureGate.Key);
            Assert.Equal(MetricType.Counter, featureGate.MetricType);
        }

        [Fact]
        public void When_BuildingFeatureGateWithFunctionAsync_Expect_FeatureGateAsyncWithReturnValue()
        {
            // Arrange
            string featureGateKey = Guid.NewGuid().ToString();

            // Act
            FeatureGateAsync<string> featureGate = new FeatureGateBuilder(featureGateKey)
                .ControlledBy(Test.BooleanFunctionAsync)
                .WhenOpened(Test.FunctionAsync<string>)
                .WhenClosed(Test.FunctionAsync<string>);

            // Assert
            Assert.Equal(featureGateKey, featureGate.Key);
            Assert.Equal(MetricType.Counter, featureGate.MetricType);
        }
    }

    private static class Test
    {
        public static bool BooleanFunction()
        {
            return new Random().NextDouble() >= 0.5;
        }

        public static Task<bool> BooleanFunctionAsync()
        {
            return Task.FromResult(new Random().NextDouble() >= 0.5);
        }

        public static void Action()
        {
            // Do nothing
        }

        public static TResult Function<TResult>()
        {
            return default;
        }

        public static Task ActionAsync()
        {
            return Task.CompletedTask;
        }

        public static Task<TResult> FunctionAsync<TResult>()
        {
            return default;
        }
    }
}
