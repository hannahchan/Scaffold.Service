namespace Scaffold.Application.UnitTests.Components.Bucket;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Scaffold.Application.Components.Bucket;
using Xunit;

public class BucketNotFoundExceptionUnitTests
{
    [Fact]
    public void When_InstantiatingBucketNotFoundExceptionWithBucketId_Expect_BucketIdInMessage()
    {
        // Arrange
        int bucketId = new Random().Next();
        BucketNotFoundException exception;

        // Act
        exception = new BucketNotFoundException(bucketId);

        // Assert
        Assert.Equal($"Bucket '{bucketId}' not found.", exception.Message);
    }

    [Fact]
    public void When_DeserializingBucketNotFoundException_Expect_SerializedBucketNotFoundException()
    {
        // Arrange
        BucketNotFoundException exception = new BucketNotFoundException(new Random().Next());

        BucketNotFoundException result;

        // Act
        using (Stream stream = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter
            {
                Binder = new CustomBinder(),
            };

            formatter.Serialize(stream, exception);
            stream.Position = 0;
            result = (BucketNotFoundException)formatter.Deserialize(stream);
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
                typeof(BucketNotFoundException).FullName,
            };

            return allowedTypes.Contains(typeName) ? Assembly.Load(assemblyName).GetType(typeName) : throw new SerializationException();
        }
    }
}
