namespace Scaffold.Domain.UnitTests.Entities
{
    using System;
    using Scaffold.Domain.Entities;
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
                Action action = () => bucket.AddItem(null);

                // Assert
                Assert.Throws<ArgumentNullException>(action);
                Assert.Empty(bucket.Items);
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
    }
}
