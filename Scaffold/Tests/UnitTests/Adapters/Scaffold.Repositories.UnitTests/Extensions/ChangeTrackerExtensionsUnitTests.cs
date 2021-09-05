namespace Scaffold.Repositories.UnitTests.Extensions
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Scaffold.Repositories.Extensions;
    using Xunit;

    public class ChangeTrackerExtensionsUnitTests
    {
        private readonly DbContextOptions<TestContext> dbContextOptions;

        public ChangeTrackerExtensionsUnitTests()
        {
            this.dbContextOptions = new DbContextOptionsBuilder<TestContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        public class UpdateChangeTimestamps : ChangeTrackerExtensionsUnitTests
        {
            [Fact]
            public void When_UpdatingChangeTrackingTimestampsOnAddedEntities_Expect_UpdatedTimestamps()
            {
                // Arrange
                using TestContext context = new TestContext(this.dbContextOptions);

                context.ModelsWithNoTimestamps.Add(new ModelWithNoTimestamps());
                context.ModelsWithCreatedAtTimestampOnly.Add(new ModelWithCreatedAtTimestampOnly());
                context.ModelsWithLastModifiedAtTimestampOnly.Add(new ModelWithLastModifiedAtTimestampOnly());
                context.ModelsWithNonNullableLastModifiedAtTimestamp.Add(new ModelWithNonNullableLastModifiedAtTimestamp());
                context.ModelsWithNullableLastModifiedAtTimestamp.Add(new ModelWithNullableLastModifiedAtTimestamp());
                context.ModelsWithStringTimestamps.Add(new ModelWithStringTimestamps());

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
            public void When_UpdatingChangeTrackingTimestampsOnModifiedEntities_Expect_UpdatedTimestamps()
            {
                // Arrange
                using TestContext context = new TestContext(this.dbContextOptions);

                context.ModelsWithNoTimestamps.Add(new ModelWithNoTimestamps());
                context.ModelsWithCreatedAtTimestampOnly.Add(new ModelWithCreatedAtTimestampOnly());
                context.ModelsWithLastModifiedAtTimestampOnly.Add(new ModelWithLastModifiedAtTimestampOnly());
                context.ModelsWithNonNullableLastModifiedAtTimestamp.Add(new ModelWithNonNullableLastModifiedAtTimestamp());
                context.ModelsWithNullableLastModifiedAtTimestamp.Add(new ModelWithNullableLastModifiedAtTimestamp());
                context.ModelsWithStringTimestamps.Add(new ModelWithStringTimestamps());

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

            public DbSet<ModelWithNoTimestamps> ModelsWithNoTimestamps => this.Set<ModelWithNoTimestamps>();

            public DbSet<ModelWithCreatedAtTimestampOnly> ModelsWithCreatedAtTimestampOnly => this.Set<ModelWithCreatedAtTimestampOnly>();

            public DbSet<ModelWithLastModifiedAtTimestampOnly> ModelsWithLastModifiedAtTimestampOnly => this.Set<ModelWithLastModifiedAtTimestampOnly>();

            public DbSet<ModelWithNonNullableLastModifiedAtTimestamp> ModelsWithNonNullableLastModifiedAtTimestamp => this.Set<ModelWithNonNullableLastModifiedAtTimestamp>();

            public DbSet<ModelWithNullableLastModifiedAtTimestamp> ModelsWithNullableLastModifiedAtTimestamp => this.Set<ModelWithNullableLastModifiedAtTimestamp>();

            public DbSet<ModelWithStringTimestamps> ModelsWithStringTimestamps => this.Set<ModelWithStringTimestamps>();
        }

#pragma warning disable S1144, S3459
        private class ModelWithNoTimestamps
        {
            public int Id { get; set; }
        }

        private class ModelWithCreatedAtTimestampOnly
        {
            public int Id { get; set; }

            public DateTime CreatedAt { get; private set; }
        }

        private class ModelWithLastModifiedAtTimestampOnly
        {
            public int Id { get; set; }

            public DateTime LastModifiedAt { get; private set; }
        }

        private class ModelWithNonNullableLastModifiedAtTimestamp
        {
            public int Id { get; set; }

            public DateTime CreatedAt { get; private set; }

            public DateTime LastModifiedAt { get; private set; }
        }

        private class ModelWithNullableLastModifiedAtTimestamp
        {
            public int Id { get; set; }

            public DateTime CreatedAt { get; private set; }

            public DateTime? LastModifiedAt { get; private set; }
        }

        private class ModelWithStringTimestamps
        {
            public int Id { get; set; }

            public string CreatedAt { get; private set; }

            public string LastModifiedAt { get; private set; }
        }
#pragma warning restore S1144, S3459
    }
}
