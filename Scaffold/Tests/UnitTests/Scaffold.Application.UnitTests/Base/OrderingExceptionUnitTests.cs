namespace Scaffold.Application.UnitTests.Base
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Scaffold.Application.Base;
    using Xunit;

    public class OrderingExceptionUnitTests
    {
        [Fact]
        public void When_InstantiatingOrderingExceptionWithMessage_Expect_OrderingExceptionWithMessage()
        {
            // Arrange
            string message = Guid.NewGuid().ToString();

            // Act
            TestException exception = new TestException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void When_InstantiatingOrderingExceptionWithMessageAndInnerException_Expect_OrderingExceptionWithMessageAndInnerException()
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
        public void When_DeserializingOrderingException_Expect_SerializedOrderingException()
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
            Assert.Equal(exception.InnerException!.Message, result.InnerException?.Message);
        }

        [Serializable]
        private class TestException : OrderingException
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
