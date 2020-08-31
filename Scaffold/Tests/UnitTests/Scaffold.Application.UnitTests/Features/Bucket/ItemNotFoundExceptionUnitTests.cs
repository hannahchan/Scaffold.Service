namespace Scaffold.Application.UnitTests.Features.Bucket
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Scaffold.Application.Features.Bucket;
    using Xunit;

    public class ItemNotFoundExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingItemNotFoundExceptionWithItemId_Expect_ItemIdInMessage()
        {
            // Arrange
            int itemId = new Random().Next();
            ItemNotFoundException exception;

            // Act
            exception = new ItemNotFoundException(itemId);

            // Assert
            Assert.Equal($"Item '{itemId}' not found.", exception.Message);
        }

        [Fact]
        public void When_DeserializingItemNotFoundException_Expect_SerializedItemNotFoundException()
        {
            // Arrange
            ItemNotFoundException exception = new ItemNotFoundException(new Random().Next());

            ItemNotFoundException result;

            // Act
            using (Stream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, exception);
                stream.Position = 0;
                result = (ItemNotFoundException)formatter.Deserialize(stream);
            }

            // Assert
            Assert.Equal(exception.Message, result.Message);
            Assert.Equal(exception.InnerException, result.InnerException);
            Assert.Null(result.InnerException);
        }
    }
}
