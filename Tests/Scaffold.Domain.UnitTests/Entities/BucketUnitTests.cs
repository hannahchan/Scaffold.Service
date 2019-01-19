namespace Scaffold.Domain.UnitTests.Entities
{
    using Xunit;

    public class BucketUnitTests
    {
        public class AddItem
        {
            [Fact]
            public void When_AddingItem_Expect_ItemAdded()
            {
            }

            [Fact]
            public void When_AddingItemMoreThanOnce_Expect_ItemAddedOnlyOnce()
            {
            }

            [Fact]
            public void When_AddingNull_Expect_ArgumentNullException()
            {
            }
        }

        public class RemoveItem
        {
            [Fact]
            public void When_RemovingItem_Expect_ItemRemoved()
            {
            }

            [Fact]
            public void When_RemovingItemNotInBucket_Expect_NoChange()
            {
            }

            [Fact]
            public void When_RemovingNull_Expect_NoChange()
            {
            }
        }
    }
}
