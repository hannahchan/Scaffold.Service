namespace Scaffold.Repositories.UnitTests.Extensions
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.EntityFrameworkCore.Metadata.Conventions;
    using Scaffold.Repositories.Extensions;
    using Xunit;

    public class ChangeTrackingTimestampExtensionsUnitTests
    {
        private readonly DbContextOptions<TestContext> dbContextOptions;

        public ChangeTrackingTimestampExtensionsUnitTests()
        {
            this.dbContextOptions = new DbContextOptionsBuilder<TestContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        public class AddChangeTrackingTimestamps : ChangeTrackingTimestampExtensionsUnitTests
        {
            [Fact]
            public void When_AddingChangeTrackingTimestamps_Expext_TimestampsAdded()
            {
                // Arrange
                using TestContext context = new TestContext(this.dbContextOptions);

                ModelBuilder modelBuilder = new ModelBuilder(ConventionSet.CreateConventionSet(context));

                modelBuilder.Entity<Mock.ModelWithNoTimestamps>();
                modelBuilder.Entity<Mock.ModelWithCreatedAtTimestampOnly>();
                modelBuilder.Entity<Mock.ModelWithLastModifiedAtTimestampOnly>();
                modelBuilder.Entity<Mock.ModelWithNonNullableLastModifiedAtTimestamp>();
                modelBuilder.Entity<Mock.ModelWithNullableLastModifiedAtTimestamp>();
                modelBuilder.Entity<Mock.ModelWithStringTimestamps>();

                // Act
                modelBuilder.AddChangeTrackingTimestamps();

                // Assert
                Assert.Collection(
                    modelBuilder.Model.GetEntityTypes(),
                    entity =>
                    {
                        Assert.Equal(typeof(Mock.ModelWithCreatedAtTimestampOnly), entity.ClrType);
                        Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                        Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    },
                    entity =>
                    {
                        Assert.Equal(typeof(Mock.ModelWithLastModifiedAtTimestampOnly), entity.ClrType);
                        Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                        Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    },
                    entity =>
                    {
                        Assert.Equal(typeof(Mock.ModelWithNoTimestamps), entity.ClrType);
                        Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                        Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    },
                    entity =>
                    {
                        Assert.Equal(typeof(Mock.ModelWithNonNullableLastModifiedAtTimestamp), entity.ClrType);
                        Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                        Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    },
                    entity =>
                    {
                        Assert.Equal(typeof(Mock.ModelWithNullableLastModifiedAtTimestamp), entity.ClrType);
                        Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                        Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    },
                    entity =>
                    {
                        Assert.Equal(typeof(Mock.ModelWithStringTimestamps), entity.ClrType);
                        Assert.Equal(typeof(string), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                        Assert.Equal(typeof(string), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    });
            }
        }

        public class UpdateChangeTimestamps : ChangeTrackingTimestampExtensionsUnitTests
        {
            [Fact]
            public void When_UpdatingChangeTrackingTimestampsOnAddedEntities_Expect_TimestampsUpdated()
            {
                // Arrange
                using TestContext context = new TestContext(this.dbContextOptions);

                context.ModelsWithNoTimestamps.Add(new Mock.ModelWithNoTimestamps());
                context.ModelsWithCreatedAtTimestampOnly.Add(new Mock.ModelWithCreatedAtTimestampOnly());
                context.ModelsWithLastModifiedAtTimestampOnly.Add(new Mock.ModelWithLastModifiedAtTimestampOnly());
                context.ModelsWithNonNullableLastModifiedAtTimestamp.Add(new Mock.ModelWithNonNullableLastModifiedAtTimestamp());
                context.ModelsWithNullableLastModifiedAtTimestamp.Add(new Mock.ModelWithNullableLastModifiedAtTimestamp());
                context.ModelsWithStringTimestamps.Add(new Mock.ModelWithStringTimestamps());

                DateTime timestamp = DateTime.UtcNow;

                // Act
                context.ChangeTracker.UpdateChangeTrackingTimestamps(timestamp);
                context.SaveChanges();

                // Assert
                Assert.Collection(
                    context.ModelsWithNoTimestamps,
                    model =>
                    {
                        Assert.NotEqual(default, model.Id);
                    });

                Assert.Collection(
                    context.ModelsWithCreatedAtTimestampOnly,
                    model =>
                    {
                        Assert.NotEqual(default, model.Id);
                        Assert.Equal(timestamp, model.CreatedAt);
                    });

                Assert.Collection(
                    context.ModelsWithLastModifiedAtTimestampOnly,
                    model =>
                    {
                        Assert.NotEqual(default, model.Id);
                        Assert.Equal(default, model.LastModifiedAt);
                    });

                Assert.Collection(
                    context.ModelsWithNonNullableLastModifiedAtTimestamp,
                    model =>
                    {
                        Assert.NotEqual(default, model.Id);
                        Assert.Equal(timestamp, model.CreatedAt);
                        Assert.Equal(timestamp, model.LastModifiedAt);
                    });

                Assert.Collection(
                    context.ModelsWithNullableLastModifiedAtTimestamp,
                    model =>
                    {
                        Assert.NotEqual(default, model.Id);
                        Assert.Equal(timestamp, model.CreatedAt);
                        Assert.Null(model.LastModifiedAt);
                    });

                Assert.Collection(
                    context.ModelsWithStringTimestamps,
                    model =>
                    {
                        Assert.NotEqual(default, model.Id);
                        Assert.Null(model.CreatedAt);
                        Assert.Null(model.LastModifiedAt);
                    });
            }

            [Fact]
            public void When_UpdatingChangeTrackingTimestampsOnModifiedEntities_Expect_TimestampsUpdated()
            {
                // Arrange
                using TestContext context = new TestContext(this.dbContextOptions);

                context.ModelsWithNoTimestamps.Add(new Mock.ModelWithNoTimestamps());
                context.ModelsWithCreatedAtTimestampOnly.Add(new Mock.ModelWithCreatedAtTimestampOnly());
                context.ModelsWithLastModifiedAtTimestampOnly.Add(new Mock.ModelWithLastModifiedAtTimestampOnly());
                context.ModelsWithNonNullableLastModifiedAtTimestamp.Add(new Mock.ModelWithNonNullableLastModifiedAtTimestamp());
                context.ModelsWithNullableLastModifiedAtTimestamp.Add(new Mock.ModelWithNullableLastModifiedAtTimestamp());
                context.ModelsWithStringTimestamps.Add(new Mock.ModelWithStringTimestamps());

                DateTime createdTimestamp = DateTime.UtcNow;
                DateTime modifiedTimestamp = createdTimestamp.AddMinutes(1);

                context.ChangeTracker.UpdateChangeTrackingTimestamps(createdTimestamp);
                context.SaveChanges();

                foreach (EntityEntry entity in context.ChangeTracker.Entries())
                {
                    entity.State = EntityState.Modified;
                }

                // Act
                context.ChangeTracker.UpdateChangeTrackingTimestamps(modifiedTimestamp);
                context.SaveChanges();

                // Assert
                Assert.Collection(
                    context.ModelsWithNoTimestamps,
                    model =>
                    {
                        Assert.NotEqual(default, model.Id);
                    });

                Assert.Collection(
                    context.ModelsWithCreatedAtTimestampOnly,
                    model =>
                    {
                        Assert.NotEqual(default, model.Id);
                        Assert.Equal(createdTimestamp, model.CreatedAt);
                    });

                Assert.Collection(
                    context.ModelsWithLastModifiedAtTimestampOnly,
                    model =>
                    {
                        Assert.NotEqual(default, model.Id);
                        Assert.Equal(modifiedTimestamp, model.LastModifiedAt);
                    });

                Assert.Collection(
                    context.ModelsWithNonNullableLastModifiedAtTimestamp,
                    model =>
                    {
                        Assert.NotEqual(default, model.Id);
                        Assert.Equal(createdTimestamp, model.CreatedAt);
                        Assert.Equal(modifiedTimestamp, model.LastModifiedAt);
                    });

                Assert.Collection(
                    context.ModelsWithNullableLastModifiedAtTimestamp,
                    model =>
                    {
                        Assert.NotEqual(default, model.Id);
                        Assert.Equal(createdTimestamp, model.CreatedAt);
                        Assert.Equal(modifiedTimestamp, model.LastModifiedAt);
                    });

                Assert.Collection(
                    context.ModelsWithStringTimestamps,
                    model =>
                    {
                        Assert.NotEqual(default, model.Id);
                        Assert.Null(model.CreatedAt);
                        Assert.Null(model.LastModifiedAt);
                    });
            }
        }

        private class TestContext : DbContext
        {
            public TestContext(DbContextOptions<TestContext> options)
                : base(options)
            {
            }

            public DbSet<Mock.ModelWithNoTimestamps> ModelsWithNoTimestamps => this.Set<Mock.ModelWithNoTimestamps>();

            public DbSet<Mock.ModelWithCreatedAtTimestampOnly> ModelsWithCreatedAtTimestampOnly => this.Set<Mock.ModelWithCreatedAtTimestampOnly>();

            public DbSet<Mock.ModelWithLastModifiedAtTimestampOnly> ModelsWithLastModifiedAtTimestampOnly => this.Set<Mock.ModelWithLastModifiedAtTimestampOnly>();

            public DbSet<Mock.ModelWithNonNullableLastModifiedAtTimestamp> ModelsWithNonNullableLastModifiedAtTimestamp => this.Set<Mock.ModelWithNonNullableLastModifiedAtTimestamp>();

            public DbSet<Mock.ModelWithNullableLastModifiedAtTimestamp> ModelsWithNullableLastModifiedAtTimestamp => this.Set<Mock.ModelWithNullableLastModifiedAtTimestamp>();

            public DbSet<Mock.ModelWithStringTimestamps> ModelsWithStringTimestamps => this.Set<Mock.ModelWithStringTimestamps>();
        }
    }
}
