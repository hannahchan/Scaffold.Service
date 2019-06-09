namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Xunit;

    public class ApplicationExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingApplicationExceptionWithMessage_Expect_ApplicationExceptionWithMessage()
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
        public void When_InstantiatingApplicationExceptionWithMessageAndInnerException_Expect_ApplicationExceptionWithMessageAndInnerException()
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
        public void When_InstantiatingApplicationExceptionWithTitleAndMessage_Expect_ApplicationExceptionWithTitleAndMessage()
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
        public void When_InstantiatingApplicationExceptionWithTitleAndMessageAndInnerException_Expect_ApplicationExceptionWithTitleAndMessageAndInnerException()
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
        public void When_DeserializingApplicationException_Expect_SerializedApplicationException()
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
            Assert.Equal(exception.Title, result.Title);
            Assert.Equal(exception.Detail, result.Detail);
            Assert.Equal(exception.Message, result.Message);
            Assert.Equal(exception.InnerException.Message, result.InnerException.Message);
        }

        [Serializable]
        private class TestException : Scaffold.Application.Exceptions.ApplicationException
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
