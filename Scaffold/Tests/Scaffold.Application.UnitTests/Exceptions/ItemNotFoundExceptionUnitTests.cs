namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using Scaffold.Application.Exceptions;
    using Xunit;

    public class ItemNotFoundExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingItemNotFoundException_Expect_TitleNotEmpty()
        {
            // Arrange
            ItemNotFoundException exception;

            // Act
            exception = new ItemNotFoundException(new Random().Next(int.MaxValue));

            // Assert
            Assert.NotEmpty(exception.Title);
        }

        [Fact]
        public void When_InstantiatingItemNotFoundException_Expect_DetailNotEmpty()
        {
            // Arrange
            ItemNotFoundException exception;

            // Act
            exception = new ItemNotFoundException(new Random().Next(int.MaxValue));

            // Assert
            Assert.NotEmpty(exception.Detail);
        }
    }
}
