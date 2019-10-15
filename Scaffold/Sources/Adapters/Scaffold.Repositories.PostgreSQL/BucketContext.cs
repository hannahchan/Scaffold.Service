namespace Scaffold.Repositories.PostgreSQL
{
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Repositories.PostgreSQL.Configurations;

    public class BucketContext : DbContext
    {
        public BucketContext(DbContextOptions<BucketContext> options)
            : base(options)
        {
        }

        public DbSet<Bucket> Buckets { get; set; } = null!;

        public DbSet<Item> Items { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new BucketConfiguration());
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
        }
    }
}
