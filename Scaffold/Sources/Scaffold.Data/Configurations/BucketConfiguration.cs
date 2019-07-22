namespace Scaffold.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Scaffold.Domain.Entities;

    public class BucketConfiguration : IEntityTypeConfiguration<Bucket>
    {
        public void Configure(EntityTypeBuilder<Bucket> builder)
        {
            builder
                .HasMany(bucket => bucket.Items);

            builder.Metadata
                .FindNavigation(nameof(Bucket.Items))
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.ToTable(nameof(Bucket));
        }
    }
}
