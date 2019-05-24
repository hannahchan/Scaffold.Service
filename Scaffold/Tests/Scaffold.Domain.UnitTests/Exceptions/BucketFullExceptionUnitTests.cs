namespace Scaffold.Domain.UnitTests.Exception
{
    using System;
    using Scaffold.Domain.Exceptions;
    using Xunit;

    public class BucketFullExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingBucketFullException_Expect_TitleNotEmpty()
        {
            // Arrange
            BucketFullException exception;

            // Act
            exception = new BucketFullException(string.Empty);

            // Assert
            Assert.Empty(exception.Detail);
            Assert.NotEmpty(exception.Title);
        }

        [Fact]
        public void When_InstantiatingBucketFullExceptionWithMessage_Expect_DetailToBeMessage()
        {
            // Arrange
            BucketFullException exception;
            string message = Guid.NewGuid().ToString();

            // Act
            exception = new BucketFullException(message);

            // Assert
            Assert.Equal(message, exception.Detail);
        }
    }
}
