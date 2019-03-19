namespace Scaffold.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Scaffold.Domain.Entities;

    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.Metadata
                .FindNavigation(nameof(Item.Bucket))
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.ToTable(nameof(Item));
        }
    }
}
