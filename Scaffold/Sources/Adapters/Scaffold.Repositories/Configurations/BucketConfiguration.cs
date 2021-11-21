#nullable disable

namespace Scaffold.Repositories.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scaffold.Domain.Aggregates.Bucket;

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
