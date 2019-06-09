namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
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

        [Fact]
        public void When_DeserializingPropertyNotFoundException_Expect_SerializedPropertyNotFoundException()
        {
            // Arrange
            PropertyNotFoundException exception = new PropertyNotFoundException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            PropertyNotFoundException result;

            // Act
            using (Stream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, exception);
                stream.Position = 0;
                result = (PropertyNotFoundException)formatter.Deserialize(stream);
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
