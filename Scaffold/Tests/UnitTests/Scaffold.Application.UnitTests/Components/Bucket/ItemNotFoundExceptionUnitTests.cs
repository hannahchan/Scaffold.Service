namespace Scaffold.Application.UnitTests.Components.Bucket;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Scaffold.Application.Components.Bucket;
using Xunit;

public class ItemNotFoundExceptionUnitTests
{
    [Fact]
    public void When_InstantiatingItemNotFoundExceptionWithItemId_Expect_ItemIdInMessage()
    {
        // Arrange
        int itemId = new Random().Next();
        ItemNotFoundException exception;

        // Act
        exception = new ItemNotFoundException(itemId);

        // Assert
        Assert.Equal($"Item '{itemId}' not found.", exception.Message);
    }

    [Fact]
    public void When_DeserializingItemNotFoundException_Expect_SerializedItemNotFoundException()
    {
        // Arrange
        ItemNotFoundException exception = new ItemNotFoundException(new Random().Next());

        ItemNotFoundException result;

        // Act
        using (Stream stream = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter
            {
                Binder = new CustomBinder(),
            };

            formatter.Serialize(stream, exception);
            stream.Position = 0;
            result = (ItemNotFoundException)formatter.Deserialize(stream);
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
                typeof(ItemNotFoundException).FullName,
            };

            return allowedTypes.Contains(typeName) ? Assembly.Load(assemblyName).GetType(typeName) : throw new SerializationException();
        }
    }
}
