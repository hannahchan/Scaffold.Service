namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Scaffold.Application.Exceptions;
    using Xunit;

    public class BucketNotFoundExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingBucketNotFoundException_Expect_TitleNotEmpty()
        {
            // Arrange
            BucketNotFoundException exception;

            // Act
            exception = new BucketNotFoundException(new Random().Next(int.MaxValue));

            // Assert
            Assert.NotEmpty(exception.Title);
        }

        [Fact]
        public void When_InstantiatingBucketNotFoundException_Expect_DetailNotEmpty()
        {
            // Arrange
            BucketNotFoundException exception;

            // Act
            exception = new BucketNotFoundException(new Random().Next(int.MaxValue));

            // Assert
            Assert.NotEmpty(exception.Detail);
        }

        [Fact]
        public void When_DeserializingBucketNotFoundException_Expect_SerializedBucketNotFoundException()
        {
            // Arrange
            BucketNotFoundException exception = new BucketNotFoundException(new Random().Next(int.MaxValue));

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
            Assert.Equal(exception.Title, result.Title);
            Assert.Equal(exception.Detail, result.Detail);
            Assert.Equal(exception.Message, result.Message);
            Assert.Equal(exception.InnerException, result.InnerException);
            Assert.Null(result.InnerException);
        }
    }
}
