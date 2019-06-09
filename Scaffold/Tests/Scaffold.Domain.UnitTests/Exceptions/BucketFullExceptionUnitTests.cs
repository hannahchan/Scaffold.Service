namespace Scaffold.Domain.UnitTests.Exception
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
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

        [Fact]
        public void When_DeserializingBucketFullException_Expect_SerializedBucketFullException()
        {
            // Arrange
            BucketFullException exception = new BucketFullException(Guid.NewGuid().ToString());

            BucketFullException result;

            // Act
            using (Stream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, exception);
                stream.Position = 0;
                result = (BucketFullException)formatter.Deserialize(stream);
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
