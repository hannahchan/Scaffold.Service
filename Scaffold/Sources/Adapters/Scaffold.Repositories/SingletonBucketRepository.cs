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

    public async Task AddAsync(Bucket bucket, CancellationToken cancellationToken = default)
    {
        using BucketContext context = await this.factory.CreateDbContextAsync(cancellationToken);

        context.Buckets.Add(bucket);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAsync(IEnumerable<Bucket> buckets, CancellationToken cancellationToken = default)
    {
        using BucketContext context = await this.factory.CreateDbContextAsync(cancellationToken);

        context.Buckets.AddRange(buckets);
        await context.SaveChangesAsync(cancellationToken);
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

    public async Task RemoveAsync(Bucket bucket, CancellationToken cancellationToken = default)
    {
        using BucketContext context = await this.factory.CreateDbContextAsync(cancellationToken);

        context.Buckets.Remove(bucket);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(IEnumerable<Bucket> buckets, CancellationToken cancellationToken = default)
    {
        using BucketContext context = await this.factory.CreateDbContextAsync(cancellationToken);

        context.Buckets.RemoveRange(buckets);
        await context.SaveChangesAsync(cancellationToken);
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

    public async Task UpdateAsync(Bucket bucket, CancellationToken cancellationToken = default)
    {
        using BucketContext context = await this.factory.CreateDbContextAsync(cancellationToken);

        context.Buckets.Update(bucket);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(IEnumerable<Bucket> buckets, CancellationToken cancellationToken = default)
    {
        using BucketContext context = await this.factory.CreateDbContextAsync(cancellationToken);

        context.Buckets.UpdateRange(buckets);
        await context.SaveChangesAsync(cancellationToken);
    }
}
