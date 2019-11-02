namespace Scaffold.Application.UnitTests.Base
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
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

        [Fact]
        public void When_DeserializingPropertyNotComparableException_Expect_SerializedPropertyNotComparableException()
        {
            // Arrange
            PropertyNotComparableException exception = new PropertyNotComparableException(Guid.NewGuid().ToString());

            PropertyNotComparableException result;

            // Act
            using (Stream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, exception);
                stream.Position = 0;
                result = (PropertyNotComparableException)formatter.Deserialize(stream);
            }

            // Assert
            Assert.Equal(exception.Title, result.Title);
            Assert.Equal(exception.Detail, result.Detail);
            Assert.Equal(exception.Message, result.Message);
            Assert.Equal(exception.InnerException, result.InnerException);
            Assert.Null(result.InnerException);
        }
    }
}
