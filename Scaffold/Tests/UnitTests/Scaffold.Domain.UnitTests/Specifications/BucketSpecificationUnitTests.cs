namespace Scaffold.Domain.UnitTests.Specifications;

using Scaffold.Domain.Aggregates.Bucket;
using Scaffold.Domain.Base;
using Scaffold.Domain.Specifications;
using Xunit;

public class BucketSpecificationUnitTests
{
    public static readonly TheoryData<Specification<Bucket>, Bucket, bool> TestSpecifications =
        new TheoryData<Specification<Bucket>, Bucket, bool>
        {
            // All Specification
            { new BucketSpecification.All(), CreateTestBucket(), true },

            // Full Specification
            { new BucketSpecification.Full(), CreateTestBucket(maxCapacity: 5, numberOfItems: 5), true },
            { new BucketSpecification.Full(), CreateTestBucket(maxCapacity: 5, numberOfItems: 3), false },
            { new BucketSpecification.Full(), CreateTestBucket(maxCapacity: 0, numberOfItems: 0), true },

            // Empty Specification
            { new BucketSpecification.Empty(), CreateTestBucket(maxCapacity: 5, numberOfItems: 5), false },
            { new BucketSpecification.Empty(), CreateTestBucket(maxCapacity: 5, numberOfItems: 3), false },
            { new BucketSpecification.Empty(), CreateTestBucket(maxCapacity: 0, numberOfItems: 0), true },

            // Max Capacity
            { new BucketSpecification.MaxCapacity(5), CreateTestBucket(maxCapacity: 4), true },
            { new BucketSpecification.MaxCapacity(5), CreateTestBucket(maxCapacity: 5), true },
            { new BucketSpecification.MaxCapacity(5), CreateTestBucket(maxCapacity: 6), false },

            // Min Capacity
            { new BucketSpecification.MinCapacity(5), CreateTestBucket(maxCapacity: 4), false },
            { new BucketSpecification.MinCapacity(5), CreateTestBucket(maxCapacity: 5), true },
            { new BucketSpecification.MinCapacity(5), CreateTestBucket(maxCapacity: 6), true },

            // Max Available Capacity
            { new BucketSpecification.MaxAvailableCapacity(5), CreateTestBucket(maxCapacity: 10, numberOfItems: 4), false },
            { new BucketSpecification.MaxAvailableCapacity(5), CreateTestBucket(maxCapacity: 10, numberOfItems: 5), true },
            { new BucketSpecification.MaxAvailableCapacity(5), CreateTestBucket(maxCapacity: 10, numberOfItems: 6), true },

            // Min Available Capacity
            { new BucketSpecification.MinAvailableCapacity(5), CreateTestBucket(maxCapacity: 10, numberOfItems: 4), true },
            { new BucketSpecification.MinAvailableCapacity(5), CreateTestBucket(maxCapacity: 10, numberOfItems: 5), true },
            { new BucketSpecification.MinAvailableCapacity(5), CreateTestBucket(maxCapacity: 10, numberOfItems: 6), false },

            // Max Used Capacity
            { new BucketSpecification.MaxUsedCapacity(5), CreateTestBucket(maxCapacity: 10, numberOfItems: 4), true },
            { new BucketSpecification.MaxUsedCapacity(5), CreateTestBucket(maxCapacity: 10, numberOfItems: 5), true },
            { new BucketSpecification.MaxUsedCapacity(5), CreateTestBucket(maxCapacity: 10, numberOfItems: 6), false },

            // Min Used Capacity
            { new BucketSpecification.MinUsedCapacity(5), CreateTestBucket(maxCapacity: 10, numberOfItems: 4), false },
            { new BucketSpecification.MinUsedCapacity(5), CreateTestBucket(maxCapacity: 10, numberOfItems: 5), true },
            { new BucketSpecification.MinUsedCapacity(5), CreateTestBucket(maxCapacity: 10, numberOfItems: 6), true },
        };

    [Theory]
    [MemberData(nameof(TestSpecifications))]
    public void When_UsingSpecification_Expect_WorkingSpecification(Specification<Bucket> specification, Bucket bucket, bool expectSatisfiedByBucket)
    {
        // Act and Assert
        if (expectSatisfiedByBucket)
        {
            Assert.True(specification.IsSatisfiedBy(bucket));
        }
        else
        {
            Assert.False(specification.IsSatisfiedBy(bucket));
        }
    }

    private static Bucket CreateTestBucket(int maxCapacity = 0, int numberOfItems = 0)
    {
        Bucket bucket = new Bucket { Size = maxCapacity };

        for (int i = 0; i < numberOfItems; i++)
        {
            bucket.AddItem(new Item());
        }

        return bucket;
    }
}
