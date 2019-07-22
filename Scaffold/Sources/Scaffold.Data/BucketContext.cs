namespace Scaffold.Data
{
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Data.Configurations;
    using Scaffold.Domain.Aggregates.Bucket;

    public class BucketContext : DbContext
    {
        public BucketContext(DbContextOptions<BucketContext> options)
            : base(options)
        {
        }

        public DbSet<Bucket> Buckets { get; set; }

        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new BucketConfiguration());
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
        }
    }
}
