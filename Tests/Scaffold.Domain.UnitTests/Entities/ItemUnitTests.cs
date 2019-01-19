namespace Scaffold.Domain.UnitTests.Entities
{
    using Scaffold.Domain.Entities;
    using Xunit;

    public class ItemUnitTests
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
                Item item = new Item();

                item.Bucket = bucket1;

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
                Item item = new Item();

                item.Bucket = bucket;

                // Act
                item.Bucket = null;

                // Assert
                Assert.DoesNotContain(item, bucket.Items);
                Assert.Null(item.Bucket);
            }
        }
    }
}
