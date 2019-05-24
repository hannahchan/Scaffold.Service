namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using Scaffold.Application.Exceptions;
    using Xunit;

    public class PropertyNotFoundExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingPropertyNotFoundException_Expect_TitleNotEmpty()
        {
            // Arrange
            PropertyNotFoundException exception;

            // Act
            exception = new PropertyNotFoundException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            // Assert
            Assert.NotEmpty(exception.Title);
        }

        [Fact]
        public void When_InstantiatingPropertyNotFoundException_Expect_DetailNotEmpty()
        {
            // Arrange
            PropertyNotFoundException exception;

            // Act
            exception = new PropertyNotFoundException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            // Assert
            Assert.NotEmpty(exception.Detail);
        }
    }
}
