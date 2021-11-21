namespace Scaffold.Domain.UnitTests.Base;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Scaffold.Domain.Base;
using Xunit;

public class DomainExceptionUnitTests
{
    [Fact]
    public void When_InstantiatingDomainExceptionWithMessage_Expect_DomainExceptionWithMessage()
    {
        // Arrange
        string message = Guid.NewGuid().ToString();

        // Act
        TestException exception = new TestException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void When_InstantiatingDomainExceptionNullMessage_Expect_DomainExceptionWithMessage()
    {
        // Act
        TestException exception = new TestException(null);

        // Assert
        Assert.NotEmpty(exception.Message);
    }

    [Fact]
    public void When_InstantiatingDomainExceptionWithMessageAndInnerException_Expect_DomainExceptionWithMessageAndInnerException()
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
    public void When_InstantiatingDomainExceptionWithNullMessageAndNullInnerException_Expect_DomainExceptionWithMessageAndNullInnerException()
    {
        // Act
        TestException exception = new TestException(null, null);

        // Assert
        Assert.NotEmpty(exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void When_DeserializingDomainException_Expect_SerializedDomainException()
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
