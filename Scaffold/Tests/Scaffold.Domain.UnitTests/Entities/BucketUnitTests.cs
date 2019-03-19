namespace Scaffold.Domain.UnitTests.Entities
{
    using System;
    using Scaffold.Domain.Entities;
    using Scaffold.Domain.Exceptions;
    using Xunit;

    public class BucketUnitTests
    {
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
                Assert.Equal(bucket, item.Bucket);
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
                Assert.Null(item.Bucket);
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
                Assert.Equal(bucket, item.Bucket);
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
                Assert.Equal(bucket, item.Bucket);
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
            public void When_SettingNegativeSize_Expect_InvalidSizeException()
            {
                // Arrange
                Bucket bucket = new Bucket();

                // Act
                Exception exception = Record.Exception(() => bucket.Size = -1);

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<InvalidSizeException>(exception);
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
            }
        }
    }
}
