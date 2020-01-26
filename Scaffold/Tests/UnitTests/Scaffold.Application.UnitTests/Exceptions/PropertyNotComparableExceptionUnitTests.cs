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
        public void When_InstantiatingPropertyNotComparableExceptionWithPropertyName_Expect_PropertyNameInMessage()
        {
            // Arrange
            string propertyName = Guid.NewGuid().ToString();
            PropertyNotComparableException exception;

            // Act
            exception = new PropertyNotComparableException(propertyName);

            // Assert
            Assert.Equal($"\"{propertyName}\" is not a comparable property.", exception.Message);
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
            Assert.Equal(exception.Message, result.Message);
            Assert.Equal(exception.InnerException, result.InnerException);
            Assert.Null(result.InnerException);
        }
    }
}
