namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Scaffold.Application.Exceptions;
    using Xunit;

    public class ItemNotFoundExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingItemNotFoundException_Expect_TitleNotEmpty()
        {
            // Arrange
            ItemNotFoundException exception;

            // Act
            exception = new ItemNotFoundException(new Random().Next(int.MaxValue));

            // Assert
            Assert.NotEmpty(exception.Title);
        }

        [Fact]
        public void When_InstantiatingItemNotFoundException_Expect_DetailNotEmpty()
        {
            // Arrange
            ItemNotFoundException exception;

            // Act
            exception = new ItemNotFoundException(new Random().Next(int.MaxValue));

            // Assert
            Assert.NotEmpty(exception.Detail);
        }

        [Fact]
        public void When_DeserializingItemNotFoundException_Expect_SerializedItemNotFoundException()
        {
            // Arrange
            ItemNotFoundException exception = new ItemNotFoundException(new Random().Next(int.MaxValue));

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
            Assert.Equal(exception.Title, result.Title);
            Assert.Equal(exception.Detail, result.Detail);
            Assert.Equal(exception.Message, result.Message);
            Assert.Equal(exception.InnerException, result.InnerException);
            Assert.Null(result.InnerException);
        }
    }
}
