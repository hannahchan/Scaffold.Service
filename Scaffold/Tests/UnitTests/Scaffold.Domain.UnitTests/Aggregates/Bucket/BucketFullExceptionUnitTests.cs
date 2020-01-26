namespace Scaffold.Domain.UnitTests.Aggregates.Bucket
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Scaffold.Domain.Aggregates.Bucket;
    using Xunit;

    public class BucketFullExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingBucketFullExceptionWithMessage_Expect_BucketFullExceptionWithMessage()
        {
            // Arrange
            BucketFullException exception;
            string message = Guid.NewGuid().ToString();

            // Act
            exception = new BucketFullException(message);

            // Assert
            Assert.Equal(message, exception.Message);
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
            Assert.Equal(exception.Message, result.Message);
            Assert.Equal(exception.InnerException, result.InnerException);
            Assert.Null(result.InnerException);
        }
    }
}
