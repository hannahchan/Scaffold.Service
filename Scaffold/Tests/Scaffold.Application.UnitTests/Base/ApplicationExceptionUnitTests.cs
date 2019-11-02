namespace Scaffold.Application.UnitTests.Base
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
            Assert.NotEmpty(exception.Title);
        }

        [Fact]
        public void When_InstantiatingApplicationExceptionNullMessage_Expect_ApplicationExceptionWithMessage()
        {
            // Arrange
            TestException exception;

            // Act
            exception = new TestException(null);

            // Assert
            Assert.NotEmpty(exception.Detail);
            Assert.NotEmpty(exception.Message);
            Assert.NotEmpty(exception.Title);
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
            Assert.NotEmpty(exception.Title);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void When_InstantiatingApplicationExceptionWithNullMessageAndNullInnerException_Expect_ApplicationExceptionWithMessageAndNullInnerException()
        {
            // Arrange
            TestException exception;

            // Act
            exception = new TestException(null, null as Exception);

            // Assert
            Assert.NotEmpty(exception.Detail);
            Assert.NotEmpty(exception.Message);
            Assert.NotEmpty(exception.Title);
            Assert.Null(exception.InnerException);
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
        public void When_InstantiatingApplicationExceptionWithNullTitleAndNullMessage_Expect_ApplicationExceptionWithTitleAndMessage()
        {
            // Arrange
            TestException exception;

            // Act
            exception = new TestException(null, null as string);

            // Assert
            Assert.NotEmpty(exception.Detail);
            Assert.NotEmpty(exception.Message);
            Assert.Equal("Application Exception", exception.Title);
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
        public void When_InstantiatingApplicationExceptionWithNullTitleAndNullMessageAndNullInnerException_Expect_ApplicationExceptionWithTitleAndMessageAndNullInnerException()
        {
            // Arrange
            TestException exception;

            // Act
            exception = new TestException(null, null, null);

            // Assert
            Assert.NotEmpty(exception.Detail);
            Assert.NotEmpty(exception.Message);
            Assert.Equal("Application Exception", exception.Title);
            Assert.Null(exception.InnerException);
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
            Assert.NotEqual(exception, result);
            Assert.Equal(exception.Title, result.Title);
            Assert.Equal(exception.Detail, result.Detail);
            Assert.Equal(exception.Message, result.Message);

            Assert.NotEqual(exception.InnerException, result.InnerException);
            Assert.Equal(exception.InnerException!.Message, result.InnerException?.Message);
        }

        [Serializable]
        private class TestException : Application.Base.ApplicationException
        {
            public TestException(string? message)
                : base(message)
            {
            }

            public TestException(string? message, Exception? innerException)
                : base(message, innerException)
            {
            }

            public TestException(string? title, string? message)
                : base(title, message)
            {
            }

            public TestException(string? title, string? message, Exception? innerException)
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
