namespace Scaffold.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Repositories.Configurations;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new BucketConfiguration());
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
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
}
