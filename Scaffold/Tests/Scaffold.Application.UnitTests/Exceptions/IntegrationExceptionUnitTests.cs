namespace Scaffold.Application.UnitTests.Exception
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Scaffold.Application.Exceptions;
    using Xunit;

    public class IntegrationExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingIntegrationExceptionWithMessageAndStatus_Expect_IntegrationExceptionWithMessageAndStatus()
        {
            // Arrange
            TestException exception;
            string message = Guid.NewGuid().ToString();
            int status = new Random().Next(int.MaxValue);

            // Act
            exception = new TestException(message, status);

            // Assert
            Assert.Equal(message, exception.Detail);
            Assert.Equal(message, exception.Message);
            Assert.Equal(status, exception.Status);
        }

        [Fact]
        public void When_InstantiatingIntegrationExceptionWithMessageAndStatusAndInnerException_Expect_IntegrationExceptionWithMessageAndStatusAndInnerException()
        {
            // Arrange
            TestException exception;

            string message = Guid.NewGuid().ToString();
            Exception innerException = new Exception();
            int status = new Random().Next(int.MaxValue);

            // Act
            exception = new TestException(message, status, innerException);

            // Assert
            Assert.Equal(message, exception.Detail);
            Assert.Equal(message, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
            Assert.Equal(status, exception.Status);
        }

        [Fact]
        public void When_InstantiatingIntegrationExceptionWithTitleAndMessageAndStatus_Expect_IntegrationExceptionWithTitleAndMessageAndStatus()
        {
            // Arrange
            TestException exception;
            string title = Guid.NewGuid().ToString();
            string message = Guid.NewGuid().ToString();
            int status = new Random().Next(int.MaxValue);

            // Act
            exception = new TestException(title, message, status);

            // Assert
            Assert.Equal(message, exception.Detail);
            Assert.Equal(message, exception.Message);
            Assert.Equal(title, exception.Title);
            Assert.Equal(status, exception.Status);
        }

        [Fact]
        public void When_InstantiatingIntegrationExceptionWithTitleAndMessageAndStatusAndInnerException_Expect_IntegrationExceptionWithTitleAndMessageAndStatusAndInnerException()
        {
            // Arrange
            TestException exception;

            string title = Guid.NewGuid().ToString();
            string message = Guid.NewGuid().ToString();
            int status = new Random().Next(int.MaxValue);
            Exception innerException = new Exception();

            // Act
            exception = new TestException(title, message, status, innerException);

            // Assert
            Assert.Equal(message, exception.Detail);
            Assert.Equal(message, exception.Message);
            Assert.Equal(title, exception.Title);
            Assert.Equal(innerException, exception.InnerException);
            Assert.Equal(status, exception.Status);
        }

        [Fact]
        public void When_DeserializingIntegrationException_Expect_SerializedIntegrationException()
        {
            // Arrange
            TestException exception = new TestException(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                new Random().Next(int.MaxValue),
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
            Assert.Equal(exception.Status, result.Status);
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

            public TestException(string title, string message, int status)
                : base(title, message, status)
            {
            }

            public TestException(string title, string message, int status, Exception innerException)
                : base(title, message, status, innerException)
            {
            }

            protected TestException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}
