namespace Scaffold.Domain.UnitTests.Exception
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Scaffold.Domain.Exceptions;
    using Xunit;

    public class DomainExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingDomainExceptionWithMessage_Expect_DomainExceptionWithMessage()
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
        public void When_InstantiatingDomainExceptionWithMessageAndInnerException_Expect_DomainExceptionWithMessageAndInnerException()
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
        public void When_InstantiatingDomainExceptionWithTitleAndMessage_Expect_DomainExceptionWithTitleAndMessage()
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
        public void When_InstantiatingDomainExceptionWithTitleAndMessageAndInnerException_Expect_DomainExceptionWithTitleAndMessageAndInnerException()
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
        public void When_DeserializingDomainException_Expect_SerializedDomainException()
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
        private class TestException : DomainException
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
