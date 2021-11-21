namespace Scaffold.Domain.UnitTests.Aggregates.Bucket;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Scaffold.Domain.Aggregates.Bucket;
using Xunit;

public class BucketFullExceptionUnitTests
{
    [Fact]
    public void When_InstantiatingBucketFullExceptionWithMessage_Expect_BucketFullExceptionWithMessage()
    {
        // Arrange
        BucketFullException exception;
        string message = Guid.NewGuid().ToString();

        // Act
        exception = new BucketFullException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void When_DeserializingBucketFullException_Expect_SerializedBucketFullException()
    {
        // Arrange
        BucketFullException exception = new BucketFullException(Guid.NewGuid().ToString());

        BucketFullException result;

        // Act
        using (Stream stream = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter
            {
                Binder = new CustomBinder(),
            };

            formatter.Serialize(stream, exception);
            stream.Position = 0;
            result = (BucketFullException)formatter.Deserialize(stream);
        }

        // Assert
        Assert.Equal(exception.Message, result.Message);
        Assert.Equal(exception.InnerException, result.InnerException);
        Assert.Null(result.InnerException);
    }

    // Types allowed to be deserialized should be restricted
    // See https://rules.sonarsource.com/csharp/RSPEC-5773
    private sealed class CustomBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            string[] allowedTypes = new string[]
            {
                typeof(BucketFullException).FullName,
            };

            return allowedTypes.Contains(typeName) ? Assembly.Load(assemblyName).GetType(typeName) : throw new SerializationException();
        }
    }
}
