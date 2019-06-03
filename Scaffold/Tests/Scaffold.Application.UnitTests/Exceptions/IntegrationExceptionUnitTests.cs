namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using Scaffold.Application.Exceptions;
    using Xunit;

    public class IntegrationExceptionUnitTests
    {
        public class Constructor
        {
            [Fact]
            public void When_InstantiatingIntegrationException_Expect_IntegrationException()
            {
                // Arrange
                TestException exception;

                // Act
                exception = new TestException();

                // Assert
                Assert.NotNull(exception);
            }

            [Fact]
            public void When_InstantiatingIntegrationExceptionWithMessage_Expect_IntegrationExceptionWithMessage()
            {
                // Arrange
                TestException exception;
                string message = Guid.NewGuid().ToString();

                // Act
                exception = new TestException(message);

                // Assert
                Assert.Equal(message, exception.Message);
            }

            [Fact]
            public void When_InstantiatingIntegrationExceptionWithMessageAndInnerException_Expect_IntegrationExceptionWithMessageAndInnerException()
            {
                // Arrange
                TestException exception;

                string message = Guid.NewGuid().ToString();
                Exception innerException = new Exception();

                // Act
                exception = new TestException(message, innerException);

                // Assert
                Assert.Equal(message, exception.Message);
                Assert.Equal(innerException, exception.InnerException);
            }
        }

        public class SetStatus
        {
            [Fact]
            public void When_SettingStatus_Expect_StatusSet()
            {
                // Arrange
                TestException exception = new TestException();
                int value = new Random().Next(int.MaxValue);

                // Act
                exception.Status = value;

                // Assert
                Assert.Equal(value, exception.Status);
            }
        }

        private class TestException : IntegrationException
        {
            public TestException()
            {
            }

            public TestException(string message)
                : base(message)
            {
            }

            public TestException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }
    }
}
