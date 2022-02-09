namespace Scaffold.Domain.Specifications;

using Scaffold.Domain.Aggregates.Bucket;
using Scaffold.Domain.Base;

public static class BucketSpecification
{
    public class All : Specification<Bucket>
    {
        public All()
            : base(bucket => true)
        {
        }
    }

    public class Full : Specification<Bucket>
    {
        public Full()
            : base(bucket => bucket.Items.Count == bucket.Size)
        {
        }
    }

    public class Empty : Specification<Bucket>
    {
        public Empty()
            : base(bucket => bucket.Items.Count == 0)
        {
        }
    }

    public class MaxCapacity : Specification<Bucket>
    {
        public MaxCapacity(int maxCapacity)
            : base(bucket => bucket.Size <= maxCapacity)
        {
        }
    }

    public class MinCapacity : Specification<Bucket>
    {
        public MinCapacity(int minCapacity)
            : base(bucket => bucket.Size >= minCapacity)
        {
        }
    }

    public class MaxAvailableCapacity : Specification<Bucket>
    {
        public MaxAvailableCapacity(int maxAvailableCapacity)
            : base(bucket => bucket.Size - bucket.Items.Count <= maxAvailableCapacity)
        {
        }
    }

    public class MinAvailableCapacity : Specification<Bucket>
    {
        public MinAvailableCapacity(int minAvailableCapacity)
            : base(bucket => bucket.Size - bucket.Items.Count >= minAvailableCapacity)
        {
        }
    }

    public class MaxUsedCapacity : Specification<Bucket>
    {
        public MaxUsedCapacity(int maxUsedCapacity)
            : base(bucket => bucket.Items.Count <= maxUsedCapacity)
        {
        }
    }

    public class MinUsedCapacity : Specification<Bucket>
    {
        public MinUsedCapacity(int minUsedCapacity)
            : base(bucket => bucket.Items.Count >= minUsedCapacity)
        {
        }
    }
}
