namespace Scaffold.Domain.UnitTests.Aggregates.Bucket
{
    using System;
    using Scaffold.Domain.Aggregates.Bucket;
    using Xunit;

    public static class ItemUnitTests
    {
        public class Constructor
        {
            [Fact]
            public void When_InstantiatingItem_Expect_ItemWithDefaultId()
            {
                // Arrange
                Item item;

                // Act
                item = new Item();

                // Assert
                Assert.Equal(default, item.Id);
            }

            [Fact]
            public void When_InstantiatingItemWithId_Expect_ItemWithId()
            {
                // Arrange
                Item item;
                int id = new Random().Next();

                // Act
                item = new Item(id);

                // Assert
                Assert.Equal(id, item.Id);
            }
        }

        public class SetDescription
        {
            [Fact]
            public void When_SettingDescription_Expect_DescriptionSet()
            {
                // Arrange
                Item item = new Item();
                string value = Guid.NewGuid().ToString();

                // Act
                item.Description = value;

                // Assert
                Assert.Equal(value, item.Description);
            }
        }

        public class SetName
        {
            [Fact]
            public void When_SettingName_Expect_NameSet()
            {
                // Arrange
                Item item = new Item();
                string value = Guid.NewGuid().ToString();

                // Act
                item.Name = value;

                // Assert
                Assert.Equal(value, item.Name);
            }
        }
    }
}
