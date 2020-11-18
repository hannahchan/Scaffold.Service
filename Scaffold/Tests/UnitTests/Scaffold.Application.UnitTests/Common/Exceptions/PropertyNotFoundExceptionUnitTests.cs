namespace Scaffold.Application.UnitTests.Common.Exceptions
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Scaffold.Application.Common.Exceptions;
    using Xunit;

    public class PropertyNotFoundExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingPropertyNotFoundExceptionWithPropertyNameAndType_Expect_PropertyNameAndTypeInMessage()
        {
            // Arrange
            string propertyName = Guid.NewGuid().ToString();
            string type = Guid.NewGuid().ToString();
            PropertyNotFoundException exception;

            // Act
            exception = new PropertyNotFoundException(propertyName, type);

            // Assert
            Assert.Equal($"\"{propertyName}\" is not a property of \"{type}\".", exception.Message);
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
            Assert.Equal(exception.Message, result.Message);
            Assert.Equal(exception.InnerException, result.InnerException);
            Assert.Null(result.InnerException);
        }
    }
}
