namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Scaffold.Application.Exceptions;
    using Xunit;

    public class NotFoundExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingNotFoundExceptionWithMessage_Expect_NotFoundExceptionWithMessage()
        {
            // Arrange
            TestException exception;
            string message = Guid.NewGuid().ToString();

            // Act
            exception = new TestException(message);

            // Assert
            Assert.Equal(message, exception.Detail);
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void When_InstantiatingNotFoundExceptionWithMessageAndInnerException_Expect_NotFoundExceptionWithMessageAndInnerException()
        {
            // Arrange
            TestException exception;

            string message = Guid.NewGuid().ToString();
            Exception innerException = new Exception();

            // Act
            exception = new TestException(message, innerException);

            // Assert
            Assert.Equal(message, exception.Detail);
            Assert.Equal(message, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void When_InstantiatingNotFoundExceptionWithTitleAndMessage_Expect_NotFoundExceptionWithTitleAndMessage()
        {
            // Arrange
            TestException exception;
            string title = Guid.NewGuid().ToString();
            string message = Guid.NewGuid().ToString();

            // Act
            exception = new TestException(title, message);

            // Assert
            Assert.Equal(message, exception.Detail);
            Assert.Equal(message, exception.Message);
            Assert.Equal(title, exception.Title);
        }

        [Fact]
        public void When_InstantiatingNotFoundExceptionWithTitleAndMessageAndInnerException_Expect_NotFoundExceptionWithTitleAndMessageAndInnerException()
        {
            // Arrange
            TestException exception;

            string title = Guid.NewGuid().ToString();
            string message = Guid.NewGuid().ToString();
            Exception innerException = new Exception();

            // Act
            exception = new TestException(title, message, innerException);

            // Assert
            Assert.Equal(message, exception.Detail);
            Assert.Equal(message, exception.Message);
            Assert.Equal(title, exception.Title);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void When_DeserializingNotFoundException_Expect_SerializedNotFoundException()
        {
            // Arrange
            TestException exception = new TestException(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                new Exception(Guid.NewGuid().ToString()));

            TestException result;

            // Act
            using (Stream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, exception);
                stream.Position = 0;
                result = (TestException)formatter.Deserialize(stream);
            }

            // Assert
            Assert.NotEqual(exception, result);
            Assert.Equal(exception.Message, result.Message);
            Assert.Equal(exception.Detail, result.Detail);
            Assert.Equal(exception.Title, result.Title);

            Assert.NotEqual(exception.InnerException, result.InnerException);
            Assert.Equal(exception.InnerException!.Message, result.InnerException?.Message);
        }

        [Serializable]
        private class TestException : NotFoundException
        {
            public TestException(string message)
                : base(message)
            {
            }

            public TestException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            public TestException(string title, string message)
                : base(title, message)
            {
            }

            public TestException(string title, string message, Exception innerException)
                : base(title, message, innerException)
            {
            }

            protected TestException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}
