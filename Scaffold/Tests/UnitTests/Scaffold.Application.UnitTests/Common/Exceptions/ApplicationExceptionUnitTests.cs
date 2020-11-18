namespace Scaffold.Application.UnitTests.Common.Exceptions
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
            string message = Guid.NewGuid().ToString();

            // Act
            TestException exception = new TestException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void When_InstantiatingApplicationExceptionNullMessage_Expect_ApplicationExceptionWithMessage()
        {
            // Act
            TestException exception = new TestException(null);

            // Assert
            Assert.NotEmpty(exception.Message);
        }

        [Fact]
        public void When_InstantiatingApplicationExceptionWithMessageAndInnerException_Expect_ApplicationExceptionWithMessageAndInnerException()
        {
            // Arrange
            string message = Guid.NewGuid().ToString();
            Exception innerException = new Exception();

            // Act
            TestException exception = new TestException(message, innerException);

            // Assert
            Assert.Equal(message, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void When_InstantiatingApplicationExceptionWithNullMessageAndNullInnerException_Expect_ApplicationExceptionWithMessageAndNullInnerException()
        {
            // Act
            TestException exception = new TestException(null, null);

            // Assert
            Assert.NotEmpty(exception.Message);
            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void When_DeserializingApplicationException_Expect_SerializedApplicationException()
        {
            // Arrange
            TestException exception = new TestException(
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

            Assert.NotEqual(exception.InnerException, result.InnerException);
            Assert.Equal(exception.InnerException.Message, result.InnerException.Message);
        }

        [Serializable]
        private class TestException : Application.Common.Exceptions.ApplicationException
        {
            public TestException(string message)
                : base(message)
            {
            }

            public TestException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            protected TestException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}
