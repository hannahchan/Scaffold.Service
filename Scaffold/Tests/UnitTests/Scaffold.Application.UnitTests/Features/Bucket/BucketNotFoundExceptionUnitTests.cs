namespace Scaffold.Application.UnitTests.Features.Bucket
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Scaffold.Application.Features.Bucket;
    using Xunit;

    public class BucketNotFoundExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingBucketNotFoundExceptionWithBucketId_Expect_BucketIdInMessage()
        {
            // Arrange
            int bucketId = new Random().Next();
            BucketNotFoundException exception;

            // Act
            exception = new BucketNotFoundException(bucketId);

            // Assert
            Assert.Equal($"Bucket '{bucketId}' not found.", exception.Message);
        }

        [Fact]
        public void When_DeserializingBucketNotFoundException_Expect_SerializedBucketNotFoundException()
        {
            // Arrange
            BucketNotFoundException exception = new BucketNotFoundException(new Random().Next());

            BucketNotFoundException result;

            // Act
            using (Stream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, exception);
                stream.Position = 0;
                result = (BucketNotFoundException)formatter.Deserialize(stream);
            }

            // Assert
            Assert.Equal(exception.Message, result.Message);
            Assert.Equal(exception.InnerException, result.InnerException);
            Assert.Null(result.InnerException);
        }
    }
}
