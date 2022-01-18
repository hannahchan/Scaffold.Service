namespace Scaffold.Repositories;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scaffold.Domain.Aggregates.Bucket;
using Scaffold.Repositories.Configurations;
using Scaffold.Repositories.Extensions;

public class BucketContext : DbContext
{
    public BucketContext(DbContextOptions<BucketContext> options)
        : base(options)
    {
    }

    protected BucketContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Bucket> Buckets => this.Set<Bucket>();

    public override int SaveChanges()
    {
        this.ChangeTracker.UpdateChangeTrackingTimestamps(DateTime.UtcNow);
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.ChangeTracker.UpdateChangeTrackingTimestamps(DateTime.UtcNow);
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new BucketConfiguration());
        modelBuilder.ApplyConfiguration(new ItemConfiguration());

        modelBuilder.AddChangeTrackingTimestamps(ChangeTrackingTimestamps.Default);
    }

    public class ReadOnly : BucketContext
    {
        public ReadOnly(DbContextOptions<ReadOnly> options)
            : base(options)
        {
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
    }
}
