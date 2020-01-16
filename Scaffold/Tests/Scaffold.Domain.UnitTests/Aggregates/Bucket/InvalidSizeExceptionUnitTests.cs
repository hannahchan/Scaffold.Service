namespace Scaffold.Domain.UnitTests.Aggregates.Bucket
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Scaffold.Domain.Aggregates.Bucket;
    using Xunit;

    public class InvalidSizeExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingInvalidSizeExceptionWithMessage_Expect_InvalidSizeExceptionWithMessage()
        {
            // Arrange
            InvalidSizeException exception;
            string message = Guid.NewGuid().ToString();

            // Act
            exception = new InvalidSizeException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void When_DeserializingInvalidSizeException_Expect_SerializedInvalidSizeException()
        {
            // Arrange
            InvalidSizeException exception = new InvalidSizeException(Guid.NewGuid().ToString());

            InvalidSizeException result;

            // Act
            using (Stream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, exception);
                stream.Position = 0;
                result = (InvalidSizeException)formatter.Deserialize(stream);
            }

            // Assert
            Assert.Equal(exception.Message, result.Message);
            Assert.Equal(exception.InnerException, result.InnerException);
            Assert.Null(result.InnerException);
        }
    }
}
