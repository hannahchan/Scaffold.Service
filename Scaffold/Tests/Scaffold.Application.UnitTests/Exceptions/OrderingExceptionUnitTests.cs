namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using Scaffold.Application.Exceptions;
    using Xunit;

    public class OrderingExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingOrderingException_Expect_OrderingException()
        {
            // Arrange
            TestException exception;

            // Act
            exception = new TestException();

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void When_InstantiatingOrderingExceptionWithMessage_Expect_OrderingExceptionWithMessage()
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
        public void When_InstantiatingOrderingExceptionWithMessageAndInnerException_Expect_OrderingExceptionWithMessageAndInnerException()
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

        private class TestException : OrderingException
        {
            public TestException()
                : base()
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
