namespace Scaffold.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Aggregates.Bucket;

public class SingletonBucketRepository : SingletonBucketReadRepository, IBucketRepository
{
    private readonly IDbContextFactory<BucketContext> factory;

    public SingletonBucketRepository(IDbContextFactory<BucketContext> factory)
        : base(factory)
    {
        this.factory = factory;
    }

    public void Add(Bucket bucket)
    {
        using BucketContext context = this.factory.CreateDbContext();

        context.Buckets.Add(bucket);
        context.SaveChanges();
    }

    public void Add(IEnumerable<Bucket> buckets)
    {
        using BucketContext context = this.factory.CreateDbContext();

        context.Buckets.AddRange(buckets);
        context.SaveChanges();
    }

    public Task AddAsync(Bucket bucket, CancellationToken cancellationToken = default)
    {
        using BucketContext context = this.factory.CreateDbContext();

        context.Buckets.Add(bucket);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task AddAsync(IEnumerable<Bucket> buckets, CancellationToken cancellationToken = default)
    {
        using BucketContext context = this.factory.CreateDbContext();

        context.Buckets.AddRange(buckets);
        return context.SaveChangesAsync(cancellationToken);
    }

    public void Remove(Bucket bucket)
    {
        using BucketContext context = this.factory.CreateDbContext();

        context.Buckets.Remove(bucket);
        context.SaveChanges();
    }

    public void Remove(IEnumerable<Bucket> buckets)
    {
        using BucketContext context = this.factory.CreateDbContext();

        context.Buckets.RemoveRange(buckets);
        context.SaveChanges();
    }

    public Task RemoveAsync(Bucket bucket, CancellationToken cancellationToken = default)
    {
        using BucketContext context = this.factory.CreateDbContext();

        context.Buckets.Remove(bucket);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task RemoveAsync(IEnumerable<Bucket> buckets, CancellationToken cancellationToken = default)
    {
        using BucketContext context = this.factory.CreateDbContext();

        context.Buckets.RemoveRange(buckets);
        return context.SaveChangesAsync(cancellationToken);
    }

    public void Update(Bucket bucket)
    {
        using BucketContext context = this.factory.CreateDbContext();

        context.Buckets.Update(bucket);
        context.SaveChanges();
    }

    public void Update(IEnumerable<Bucket> buckets)
    {
        using BucketContext context = this.factory.CreateDbContext();

        context.Buckets.UpdateRange(buckets);
        context.SaveChanges();
    }

    public Task UpdateAsync(Bucket bucket, CancellationToken cancellationToken = default)
    {
        using BucketContext context = this.factory.CreateDbContext();

        context.Buckets.Update(bucket);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(IEnumerable<Bucket> buckets, CancellationToken cancellationToken = default)
    {
        using BucketContext context = this.factory.CreateDbContext();

        context.Buckets.UpdateRange(buckets);
        return context.SaveChangesAsync(cancellationToken);
    }
}
