namespace Scaffold.Domain.UnitTests.Aggregates.Bucket
{
    using System;
    using Scaffold.Domain.Aggregates.Bucket;
    using Xunit;

    public static class BucketUnitTests
    {
        public class Constructor
        {
            [Fact]
            public void When_InstantiatingBucket_Expect_BucketWithDefaultId()
            {
                // Arrange
                Bucket bucket;

                // Act
                bucket = new Bucket();

                // Assert
                Assert.Equal(default, bucket.Id);
            }

            [Fact]
            public void When_InstantiatingBucketWithId_Expect_BucketWithId()
            {
                // Arrange
                Bucket bucket;
                int id = new Random().Next(int.MaxValue);

                // Act
                bucket = new Bucket(id);

                // Assert
                Assert.Equal(id, bucket.Id);
            }
        }

        public class AddItem
        {
            [Fact]
            public void When_AddingItem_Expect_ItemAdded()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item();

                // Act
                bucket.AddItem(item);

                // Assert
                Assert.Contains(item, bucket.Items);
            }

            [Fact]
            public void When_AddingItemMoreThanOnce_Expect_ItemAddedOnlyOnce()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item();

                // Act
                bucket.AddItem(item);
                bucket.AddItem(item);

                // Assert
                Assert.Equal(1, bucket.Items.Count);
            }

            [Fact]
            public void When_AddingNull_Expect_ArgumentNullException()
            {
                // Arrange
                Bucket bucket = new Bucket();

                // Act
                Exception exception = Record.Exception(() => bucket.AddItem(null));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Empty(bucket.Items);
            }

            [Fact]
            public void When_AddingItemToFullBucket_ExpectBucketFullException()
            {
                // Arrange
                Bucket bucket = new Bucket { Size = 3 };
                bucket.AddItem(new Item());
                bucket.AddItem(new Item());
                bucket.AddItem(new Item());

                // Act
                Exception exception = Record.Exception(() => bucket.AddItem(new Item()));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<BucketFullException>(exception);
                Assert.Equal(bucket.Size, bucket.Items.Count);
                Assert.NotEmpty(exception.Message);
            }

            [Fact]
            public void When_AddingItemsToFillBucket_ExpectFullBucket()
            {
                // Arrange
                Bucket bucket = new Bucket { Size = 3 };

                // Act
                Exception exception = Record.Exception(() =>
                {
                    bucket.AddItem(new Item());
                    bucket.AddItem(new Item());
                    bucket.AddItem(new Item());
                });

                // Assert
                Assert.Null(exception);
                Assert.Equal(bucket.Size, bucket.Items.Count);
            }
        }

        public class RemoveItem
        {
            [Fact]
            public void When_RemovingItem_Expect_ItemRemoved()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item();

                bucket.AddItem(item);

                // Act
                bucket.RemoveItem(item);

                // Assert
                Assert.DoesNotContain(item, bucket.Items);
            }

            [Fact]
            public void When_RemovingItemNotInBucket_Expect_NoChange()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item();

                bucket.AddItem(item);

                // Act
                bucket.RemoveItem(new Item());

                // Assert
                Assert.Contains(item, bucket.Items);
                Assert.Equal(1, bucket.Items.Count);
            }

            [Fact]
            public void When_RemovingNull_Expect_NoChange()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item();

                bucket.AddItem(item);

                // Act
                bucket.RemoveItem(null);

                // Assert
                Assert.Contains(item, bucket.Items);
                Assert.Equal(1, bucket.Items.Count);
            }
        }

        public class SetDescription
        {
            [Fact]
            public void When_SettingDescription_Expect_DescriptionSet()
            {
                // Arrange
                Bucket bucket = new Bucket();
                string value = Guid.NewGuid().ToString();

                // Act
                bucket.Description = value;

                // Assert
                Assert.Equal(value, bucket.Description);
            }
        }

        public class SetName
        {
            [Fact]
            public void When_SettingName_Expect_NameSet()
            {
                // Arrange
                Bucket bucket = new Bucket();
                string value = Guid.NewGuid().ToString();

                // Act
                bucket.Name = value;

                // Assert
                Assert.Equal(value, bucket.Name);
            }
        }

        public class SetSize
        {
            [Fact]
            public void When_SettingSize_Expect_SizeSet()
            {
                // Arrange
                Bucket bucket = new Bucket();
                int newValue = new Random().Next(int.MaxValue);

                // Act
                bucket.Size = newValue;

                // Assert
                Assert.Equal(newValue, bucket.Size);
            }

            [Fact]
            public void When_SettingSizeToZero_Expect_SizeSetToZero()
            {
                // Arrange
                Bucket bucket = new Bucket();
                int newValue = 0;

                // Act
                bucket.Size = newValue;

                // Assert
                Assert.Equal(newValue, bucket.Size);
            }

            [Fact]
            public void When_SettingSizeToNumberOfItemsInBucket_Expect_SizeSetToNumberOfItemsInBucket()
            {
                // Arrange
                Bucket bucket = new Bucket();
                bucket.AddItem(new Item());
                int newValue = 1;

                // Act
                bucket.Size = newValue;

                // Assert
                Assert.Equal(newValue, bucket.Size);
            }

            [Fact]
            public void When_SettingNegativeSize_Expect_InvalidSizeException()
            {
                // Arrange
                Bucket bucket = new Bucket();

                // Act
                Exception exception = Record.Exception(() => bucket.Size = -1);

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<InvalidSizeException>(exception);
                Assert.NotEmpty(exception.Message);
            }

            [Fact]
            public void When_SettingSizeToLessThanNumberOfItemsInBucket_Expect_InvalidSizeException()
            {
                // Arrange
                Bucket bucket = new Bucket();
                bucket.AddItem(new Item());

                // Act
                Exception exception = Record.Exception(() => bucket.Size = 0);

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<InvalidSizeException>(exception);
                Assert.NotEmpty(exception.Message);
            }
        }
    }
}