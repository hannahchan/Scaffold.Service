namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using Scaffold.Application.Exceptions;
    using Xunit;

    public class PropertyNotComparableExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingPropertyNotComparableException_Expect_TitleNotEmpty()
        {
            // Arrange
            PropertyNotComparableException exception;

            // Act
            exception = new PropertyNotComparableException(Guid.NewGuid().ToString());

            // Assert
            Assert.NotEmpty(exception.Title);
        }

        [Fact]
        public void When_InstantiatingPropertyNotComparableException_Expect_DetailNotEmpty()
        {
            // Arrange
            PropertyNotComparableException exception;

            // Act
            exception = new PropertyNotComparableException(Guid.NewGuid().ToString());

            // Assert
            Assert.NotEmpty(exception.Detail);
        }
    }
}
