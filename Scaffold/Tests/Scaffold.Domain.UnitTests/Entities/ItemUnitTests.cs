namespace Scaffold.Domain.UnitTests.Entities
{
    using System;
    using Scaffold.Domain.Entities;
    using Scaffold.Domain.Exceptions;
    using Xunit;

    public static class ItemUnitTests
    {
        public class SetBucket
        {
            [Fact]
            public void When_SettingBucketToBucket_Expect_BucketSetToBucket()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item();

                // Act
                item.Bucket = bucket;

                // Assert
                Assert.Contains(item, bucket.Items);
                Assert.Equal(bucket, item.Bucket);
            }

            [Fact]
            public void When_SettingBucketToAnotherBucket_Expect_BucketSetToOtherBucket()
            {
                // Arrange
                Bucket bucket1 = new Bucket();
                Bucket bucket2 = new Bucket();
                Item item = new Item { Bucket = bucket1 };

                // Act
                item.Bucket = bucket2;

                // Assert
                Assert.Contains(item, bucket2.Items);
                Assert.Empty(bucket1.Items);
                Assert.Equal(bucket2, item.Bucket);
            }

            [Fact]
            public void When_SettingBucketToNull_Expect_BucketSetToNull()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item { Bucket = bucket };

                // Act
                item.Bucket = null;

                // Assert
                Assert.DoesNotContain(item, bucket.Items);
                Assert.Null(item.Bucket);
            }

            [Fact]
            public void When_SettingBucketToBucketThatIsFull_Expect_BucketFullException()
            {
                // Arrange
                Bucket bucket = new Bucket { Size = 3 };
                bucket.AddItem(new Item());
                bucket.AddItem(new Item());
                bucket.AddItem(new Item());

                // Act
                Exception exception = Record.Exception(() => new Item().Bucket = bucket);

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<BucketFullException>(exception);
                Assert.NotEmpty(exception.Message);
            }

            [Fact]
            public void When_SettingBucketOnItemsToFillBucket_ExpectFullBucket()
            {
                // Arrange
                Bucket bucket = new Bucket { Size = 3 };

                // Act
                Exception exception = Record.Exception(() =>
                {
                    new Item().Bucket = bucket;
                    new Item().Bucket = bucket;
                    new Item().Bucket = bucket;
                });

                // Assert
                Assert.Null(exception);
                Assert.Equal(bucket.Size, bucket.Items.Count);
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

        public class SetId
        {
            [Fact]
            public void When_SettingId_Expect_IdSet()
            {
                // Arrange
                Item item = new Item();
                int value = new Random().Next(int.MaxValue);

                // Act
                item.Id = value;

                // Assert
                Assert.Equal(value, item.Id);
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
