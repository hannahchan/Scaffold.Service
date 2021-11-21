namespace Scaffold.Application.UnitTests.Common.Exceptions;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Scaffold.Application.Common.Exceptions;
using Xunit;

public class NotFoundExceptionUnitTests
{
    [Fact]
    public void When_InstantiatingNotFoundExceptionWithMessage_Expect_NotFoundExceptionWithMessage()
    {
        // Arrange
        string message = Guid.NewGuid().ToString();

        // Act
        TestException exception = new TestException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void When_InstantiatingNotFoundExceptionWithMessageAndInnerException_Expect_NotFoundExceptionWithMessageAndInnerException()
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
    public void When_DeserializingNotFoundException_Expect_SerializedNotFoundException()
    {
        // Arrange
        TestException exception = new TestException(
            Guid.NewGuid().ToString(),
            new Exception(Guid.NewGuid().ToString()));

        TestException result;

        // Act
        using (Stream stream = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter
            {
                Binder = new CustomBinder(),
            };

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

        protected TestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    // Types allowed to be deserialized should be restricted
    // See https://rules.sonarsource.com/csharp/RSPEC-5773
    private sealed class CustomBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            string[] allowedTypes = new string[]
            {
                typeof(TestException).FullName,
                typeof(Exception).FullName,
            };

            return allowedTypes.Contains(typeName) ? Assembly.Load(assemblyName).GetType(typeName) : throw new SerializationException();
        }
    }
}
