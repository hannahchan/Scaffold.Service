namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using Scaffold.Application.Exceptions;
    using Xunit;

    public class BucketNotFoundExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingBucketNotFoundException_Expect_TitleNotEmpty()
        {
            // Arrange
            BucketNotFoundException exception;

            // Act
            exception = new BucketNotFoundException(new Random().Next(int.MaxValue));

            // Assert
            Assert.NotEmpty(exception.Title);
        }

        [Fact]
        public void When_InstantiatingBucketNotFoundException_Expect_DetailNotEmpty()
        {
            // Arrange
            BucketNotFoundException exception;

            // Act
            exception = new BucketNotFoundException(new Random().Next(int.MaxValue));

            // Assert
            Assert.NotEmpty(exception.Detail);
        }
    }
}
