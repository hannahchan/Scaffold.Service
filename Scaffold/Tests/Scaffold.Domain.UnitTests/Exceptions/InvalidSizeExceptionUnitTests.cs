namespace Scaffold.Domain.UnitTests.Exception
{
    using System;
    using Scaffold.Domain.Exceptions;
    using Xunit;

    public class InvalidSizeExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingInvalidSizeException_Expect_TitleNotEmpty()
        {
            // Arrange
            InvalidSizeException exception;

            // Act
            exception = new InvalidSizeException(string.Empty);

            // Assert
            Assert.Empty(exception.Detail);
            Assert.NotEmpty(exception.Title);
        }

        [Fact]
        public void When_InstantiatingInvalidSizeExceptionWithMessage_Expect_DetailToBeMessage()
        {
            // Arrange
            InvalidSizeException exception;
            string message = Guid.NewGuid().ToString();

            // Act
            exception = new InvalidSizeException(message);

            // Assert
            Assert.Equal(message, exception.Detail);
        }
    }
}
