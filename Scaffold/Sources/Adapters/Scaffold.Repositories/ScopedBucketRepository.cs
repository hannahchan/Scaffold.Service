namespace Scaffold.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Aggregates.Bucket;

public class ScopedBucketRepository : ScopedBucketReadRepository, IBucketRepository
{
    private readonly BucketContext context;

    public ScopedBucketRepository(BucketContext context)
        : base(context)
    {
        this.context = context;
    }

    public void Add(Bucket bucket)
    {
        this.context.Buckets.Add(bucket);
        this.context.SaveChanges();
    }

    public void Add(IEnumerable<Bucket> buckets)
    {
        this.context.Buckets.AddRange(buckets);
        this.context.SaveChanges();
    }

    public Task AddAsync(Bucket bucket, CancellationToken cancellationToken = default)
    {
        this.context.Buckets.Add(bucket);
        return this.context.SaveChangesAsync(cancellationToken);
    }

    public Task AddAsync(IEnumerable<Bucket> buckets, CancellationToken cancellationToken = default)
    {
        this.context.Buckets.AddRange(buckets);
        return this.context.SaveChangesAsync(cancellationToken);
    }

    public void Remove(Bucket bucket)
    {
        this.context.Buckets.Remove(bucket);
        this.context.SaveChanges();
    }

    public void Remove(IEnumerable<Bucket> buckets)
    {
        this.context.Buckets.RemoveRange(buckets);
        this.context.SaveChanges();
    }

    public Task RemoveAsync(Bucket bucket, CancellationToken cancellationToken = default)
    {
        this.context.Buckets.Remove(bucket);
        return this.context.SaveChangesAsync(cancellationToken);
    }

    public Task RemoveAsync(IEnumerable<Bucket> buckets, CancellationToken cancellationToken = default)
    {
        this.context.Buckets.RemoveRange(buckets);
        return this.context.SaveChangesAsync(cancellationToken);
    }

    public void Update(Bucket bucket)
    {
        this.context.Buckets.Update(bucket);
        this.context.SaveChanges();
    }

    public void Update(IEnumerable<Bucket> buckets)
    {
        this.context.Buckets.UpdateRange(buckets);
        this.context.SaveChanges();
    }

    public Task UpdateAsync(Bucket bucket, CancellationToken cancellationToken = default)
    {
        this.context.Buckets.Update(bucket);
        return this.context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(IEnumerable<Bucket> buckets, CancellationToken cancellationToken = default)
    {
        this.context.Buckets.UpdateRange(buckets);
        return this.context.SaveChangesAsync(cancellationToken);
    }
}
