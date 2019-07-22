namespace Scaffold.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Scaffold.Domain.Entities;

    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.Property<int>($"{nameof(Bucket)}Id");

            builder.Property(item => item.Id)
                .ValueGeneratedOnAdd();

            builder.HasKey($"{nameof(Bucket)}Id", nameof(Item.Id));

            builder.ToTable(nameof(Item));
        }
    }
}
