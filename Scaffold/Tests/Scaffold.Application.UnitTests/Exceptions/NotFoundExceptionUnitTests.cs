namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using Scaffold.Application.Exceptions;
    using Xunit;

    public class NotFoundExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingNotFoundException_Expect_NotFoundException()
        {
            // Arrange
            TestException exception;

            // Act
            exception = new TestException();

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void When_InstantiatingNotFoundExceptionWithMessage_Expect_NotFoundExceptionWithMessage()
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
        public void When_InstantiatingNotFoundExceptionWithMessageAndInnerException_Expect_NotFoundExceptionWithMessageAndInnerException()
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

        private class TestException : NotFoundException
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
