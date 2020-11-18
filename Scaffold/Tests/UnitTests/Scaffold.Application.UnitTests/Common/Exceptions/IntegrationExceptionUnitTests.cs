namespace Scaffold.Application.UnitTests.Common.Exceptions
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Scaffold.Application.Common.Exceptions;
    using Xunit;

    public class IntegrationExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingIntegrationExceptionWithMessageAndStatus_Expect_IntegrationExceptionWithMessageAndStatus()
        {
            // Arrange
            string message = Guid.NewGuid().ToString();
            int status = new Random().Next();

            // Act
            TestException exception = new TestException(message, status);

            // Assert
            Assert.Equal(message, exception.Message);
            Assert.Equal(status, exception.Status);
        }

        [Fact]
        public void When_InstantiatingIntegrationExceptionWithMessageAndStatusAndInnerException_Expect_IntegrationExceptionWithMessageAndStatusAndInnerException()
        {
            // Arrange
            string message = Guid.NewGuid().ToString();
            int status = new Random().Next();
            Exception innerException = new Exception();

            // Act
            TestException exception = new TestException(message, status, innerException);

            // Assert
            Assert.Equal(message, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
            Assert.Equal(status, exception.Status);
        }

        [Fact]
        public void When_DeserializingIntegrationException_Expect_SerializedIntegrationException()
        {
            // Arrange
            TestException exception = new TestException(
                Guid.NewGuid().ToString(),
                new Random().Next(),
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
        private class TestException : IntegrationException
        {
            public TestException(string message, int status)
                : base(message, status)
            {
            }

            public TestException(string message, int status, Exception innerException)
                : base(message, status, innerException)
            {
            }

            protected TestException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}
