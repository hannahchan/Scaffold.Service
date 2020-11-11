namespace Scaffold.Repositories.PostgreSQL
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;

    public class BucketRepository : BucketReadRepository, IBucketRepository
    {
        private readonly BucketContext context;

        public BucketRepository(BucketContext context)
            : base(context)
        {
            this.context = context;
        }

        public void Add(Bucket bucket)
        {
            if (bucket is null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Buckets.Add(bucket);
            this.context.SaveChanges();
        }

        public Task AddAsync(Bucket bucket, CancellationToken cancellationToken = default)
        {
            if (bucket is null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Buckets.Add(bucket);
            return this.context.SaveChangesAsync(cancellationToken);
        }

        public void Remove(Bucket bucket)
        {
            if (bucket is null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Buckets.Remove(bucket);
            this.context.SaveChanges();
        }

        public Task RemoveAsync(Bucket bucket, CancellationToken cancellationToken = default)
        {
            if (bucket is null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Buckets.Remove(bucket);
            return this.context.SaveChangesAsync(cancellationToken);
        }

        public void Update(Bucket bucket)
        {
            if (bucket is null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Buckets.Update(bucket);
            this.context.SaveChanges();
        }

        public Task UpdateAsync(Bucket bucket, CancellationToken cancellationToken = default)
        {
            if (bucket is null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Buckets.Update(bucket);
            return this.context.SaveChangesAsync(cancellationToken);
        }
    }
}
