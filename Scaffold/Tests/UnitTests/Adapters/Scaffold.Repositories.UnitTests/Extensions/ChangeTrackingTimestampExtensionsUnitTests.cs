namespace Scaffold.Repositories.UnitTests.Extensions;

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
        public void When_AddingCreatedAtOnlyTimestamp_Expext_CreatedAtTimestampAdded()
        {
            // Arrange
            using TestContext context = new TestContext(this.dbContextOptions);

            ModelBuilder modelBuilder = new ModelBuilder(ConventionSet.CreateConventionSet(context));

            modelBuilder.Entity<Mock.ModelWithAllTimestamps>();
            modelBuilder.Entity<Mock.ModelWithNoTimestamps>();
            modelBuilder.Entity<Mock.ModelWithNonNullableTimestamps>();

            modelBuilder.Entity<Mock.ModelWithCreatedAtTimestampOnly>();
            modelBuilder.Entity<Mock.ModelWithLastModifiedAtTimestampOnly>();
            modelBuilder.Entity<Mock.ModelWithDeletedAtTimestampOnly>();
            modelBuilder.Entity<Mock.ModelWithLastModifiedAtNonNullableTimestampOnly>();
            modelBuilder.Entity<Mock.ModelWithDeletedAtNonNullableTimestampOnly>();

            modelBuilder.Entity<Mock.ModelWithStringTimestamps>();

            // Act
            modelBuilder.AddChangeTrackingTimestamps(ChangeTrackingTimestamps.CreatedAt);

            // Assert
            Assert.Collection(
                modelBuilder.Model.GetEntityTypes(),
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithAllTimestamps), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithCreatedAtTimestampOnly), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.LastModifiedAt));
                    Assert.Null(entity.FindProperty(PropertyName.DeletedAt));
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithDeletedAtNonNullableTimestampOnly), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.LastModifiedAt));
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithDeletedAtTimestampOnly), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.LastModifiedAt));
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithLastModifiedAtNonNullableTimestampOnly), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.DeletedAt));
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithLastModifiedAtTimestampOnly), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.DeletedAt));
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithNoTimestamps), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.LastModifiedAt));
                    Assert.Null(entity.FindProperty(PropertyName.DeletedAt));
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithNonNullableTimestamps), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithStringTimestamps), entity.ClrType);
                    Assert.Equal(typeof(string), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Equal(typeof(string), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Equal(typeof(string), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Null(entity.GetQueryFilter());
                });
        }

        [Fact]
        public void When_AddingLastModifiedAtOnlyTimestamp_Expext_LastModifiedAtTimestampAdded()
        {
            // Arrange
            using TestContext context = new TestContext(this.dbContextOptions);

            ModelBuilder modelBuilder = new ModelBuilder(ConventionSet.CreateConventionSet(context));

            modelBuilder.Entity<Mock.ModelWithAllTimestamps>();
            modelBuilder.Entity<Mock.ModelWithNoTimestamps>();
            modelBuilder.Entity<Mock.ModelWithNonNullableTimestamps>();

            modelBuilder.Entity<Mock.ModelWithCreatedAtTimestampOnly>();
            modelBuilder.Entity<Mock.ModelWithLastModifiedAtTimestampOnly>();
            modelBuilder.Entity<Mock.ModelWithDeletedAtTimestampOnly>();
            modelBuilder.Entity<Mock.ModelWithLastModifiedAtNonNullableTimestampOnly>();
            modelBuilder.Entity<Mock.ModelWithDeletedAtNonNullableTimestampOnly>();

            modelBuilder.Entity<Mock.ModelWithStringTimestamps>();

            // Act
            modelBuilder.AddChangeTrackingTimestamps(ChangeTrackingTimestamps.LastModifiedAt);

            // Assert
            Assert.Collection(
                modelBuilder.Model.GetEntityTypes(),
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithAllTimestamps), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithCreatedAtTimestampOnly), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.DeletedAt));
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithDeletedAtNonNullableTimestampOnly), entity.ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.CreatedAt));
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithDeletedAtTimestampOnly), entity.ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.CreatedAt));
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithLastModifiedAtNonNullableTimestampOnly), entity.ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.CreatedAt));
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.DeletedAt));
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithLastModifiedAtTimestampOnly), entity.ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.CreatedAt));
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.DeletedAt));
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithNoTimestamps), entity.ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.CreatedAt));
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.DeletedAt));
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithNonNullableTimestamps), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Null(entity.GetQueryFilter());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithStringTimestamps), entity.ClrType);
                    Assert.Equal(typeof(string), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Equal(typeof(string), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Equal(typeof(string), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Null(entity.GetQueryFilter());
                });
        }

        [Fact]
        public void When_AddingDeleteAtOnlyTimestamp_Expect_DeletedAtTimestampAdded()
        {
            // Arrange
            using TestContext context = new TestContext(this.dbContextOptions);

            ModelBuilder modelBuilder = new ModelBuilder(ConventionSet.CreateConventionSet(context));

            modelBuilder.Entity<Mock.ModelWithAllTimestamps>();
            modelBuilder.Entity<Mock.ModelWithNoTimestamps>();
            modelBuilder.Entity<Mock.ModelWithNonNullableTimestamps>();

            modelBuilder.Entity<Mock.ModelWithCreatedAtTimestampOnly>();
            modelBuilder.Entity<Mock.ModelWithLastModifiedAtTimestampOnly>();
            modelBuilder.Entity<Mock.ModelWithDeletedAtTimestampOnly>();
            modelBuilder.Entity<Mock.ModelWithLastModifiedAtNonNullableTimestampOnly>();
            modelBuilder.Entity<Mock.ModelWithDeletedAtNonNullableTimestampOnly>();

            modelBuilder.Entity<Mock.ModelWithStringTimestamps>();

            // Act
            modelBuilder.AddChangeTrackingTimestamps(ChangeTrackingTimestamps.DeletedAt);

            // Assert
            Assert.Collection(
                modelBuilder.Model.GetEntityTypes(),
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithAllTimestamps), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Equal("entity => (Property(entity, \"DeletedAt\") == null)", entity.GetQueryFilter().ToString());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithCreatedAtTimestampOnly), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.LastModifiedAt));
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Equal("entity => (Property(entity, \"DeletedAt\") == null)", entity.GetQueryFilter().ToString());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithDeletedAtNonNullableTimestampOnly), entity.ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.CreatedAt));
                    Assert.Null(entity.FindProperty(PropertyName.LastModifiedAt));
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Equal("entity => (Property(entity, \"DeletedAt\") == default(DateTime))", entity.GetQueryFilter().ToString());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithDeletedAtTimestampOnly), entity.ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.CreatedAt));
                    Assert.Null(entity.FindProperty(PropertyName.LastModifiedAt));
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Equal("entity => (Property(entity, \"DeletedAt\") == null)", entity.GetQueryFilter().ToString());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithLastModifiedAtNonNullableTimestampOnly), entity.ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.CreatedAt));
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Equal("entity => (Property(entity, \"DeletedAt\") == null)", entity.GetQueryFilter().ToString());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithLastModifiedAtTimestampOnly), entity.ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.CreatedAt));
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Equal("entity => (Property(entity, \"DeletedAt\") == null)", entity.GetQueryFilter().ToString());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithNoTimestamps), entity.ClrType);
                    Assert.Null(entity.FindProperty(PropertyName.CreatedAt));
                    Assert.Null(entity.FindProperty(PropertyName.LastModifiedAt));
                    Assert.Equal(typeof(DateTime?), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Equal("entity => (Property(entity, \"DeletedAt\") == null)", entity.GetQueryFilter().ToString());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithNonNullableTimestamps), entity.ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Equal(typeof(DateTime), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Equal("entity => (Property(entity, \"DeletedAt\") == default(DateTime))", entity.GetQueryFilter().ToString());
                },
                entity =>
                {
                    Assert.Equal(typeof(Mock.ModelWithStringTimestamps), entity.ClrType);
                    Assert.Equal(typeof(string), entity.FindProperty(PropertyName.CreatedAt).ClrType);
                    Assert.Equal(typeof(string), entity.FindProperty(PropertyName.LastModifiedAt).ClrType);
                    Assert.Equal(typeof(string), entity.FindProperty(PropertyName.DeletedAt).ClrType);
                    Assert.Null(entity.GetQueryFilter());
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

            context.ModelsWithAllTimestamps.Add(new Mock.ModelWithAllTimestamps() { LastModifiedAt = DateTime.Now });
            context.ModelsWithNoTimestamps.Add(new Mock.ModelWithNoTimestamps());
            context.ModelsWithNonNullableTimestamps.Add(new Mock.ModelWithNonNullableTimestamps());

            context.ModelsWithCreatedAtTimestampOnly.Add(new Mock.ModelWithCreatedAtTimestampOnly());
            context.ModelsWithLastModifiedAtTimestampOnly.Add(new Mock.ModelWithLastModifiedAtTimestampOnly());
            context.ModelsWithDeletedAtTimestampOnly.Add(new Mock.ModelWithDeletedAtTimestampOnly());
            context.ModelsWithLastModifiedAtNonNullableTimestampOnly.Add(new Mock.ModelWithLastModifiedAtNonNullableTimestampOnly());
            context.ModelsWithDeletedAtNonNullableTimestampOnly.Add(new Mock.ModelWithDeletedAtNonNullableTimestampOnly());

            context.ModelsWithStringTimestamps.Add(new Mock.ModelWithStringTimestamps());

            DateTime createdTimestamp = DateTime.UtcNow;

            // Act
            context.ChangeTracker.UpdateChangeTrackingTimestamps(createdTimestamp);
            context.SaveChanges();

            // Assert
            Assert.Collection(
                context.ModelsWithAllTimestamps,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Equal(createdTimestamp, model.CreatedAt);
                    Assert.Null(model.LastModifiedAt);
                    Assert.Null(model.DeletedAt);
                });

            Assert.Collection(
                context.ModelsWithNoTimestamps,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                });

            Assert.Collection(
                context.ModelsWithNonNullableTimestamps,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Equal(createdTimestamp, model.CreatedAt);
                    Assert.Equal(createdTimestamp, model.LastModifiedAt);
                    Assert.Equal(default, model.DeletedAt);
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
                    Assert.Null(model.LastModifiedAt);
                });

            Assert.Collection(
                context.ModelsWithDeletedAtTimestampOnly,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Null(model.DeletedAt);
                });

            Assert.Collection(
                context.ModelsWithLastModifiedAtNonNullableTimestampOnly,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Equal(createdTimestamp, model.LastModifiedAt);
                });

            Assert.Collection(
                context.ModelsWithDeletedAtNonNullableTimestampOnly,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Equal(default, model.DeletedAt);
                });

            Assert.Collection(
                context.ModelsWithStringTimestamps,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Null(model.CreatedAt);
                    Assert.Null(model.LastModifiedAt);
                    Assert.Null(model.DeletedAt);
                });
        }

        [Fact]
        public void When_UpdatingChangeTrackingTimestampsOnModifiedEntities_Expect_TimestampsUpdated()
        {
            // Arrange
            using TestContext context = new TestContext(this.dbContextOptions);

            context.ModelsWithAllTimestamps.Add(new Mock.ModelWithAllTimestamps());
            context.ModelsWithNoTimestamps.Add(new Mock.ModelWithNoTimestamps());
            context.ModelsWithNonNullableTimestamps.Add(new Mock.ModelWithNonNullableTimestamps());

            context.ModelsWithCreatedAtTimestampOnly.Add(new Mock.ModelWithCreatedAtTimestampOnly());
            context.ModelsWithLastModifiedAtTimestampOnly.Add(new Mock.ModelWithLastModifiedAtTimestampOnly());
            context.ModelsWithDeletedAtTimestampOnly.Add(new Mock.ModelWithDeletedAtTimestampOnly());
            context.ModelsWithLastModifiedAtNonNullableTimestampOnly.Add(new Mock.ModelWithLastModifiedAtNonNullableTimestampOnly());
            context.ModelsWithDeletedAtNonNullableTimestampOnly.Add(new Mock.ModelWithDeletedAtNonNullableTimestampOnly());

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
                context.ModelsWithAllTimestamps,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Equal(createdTimestamp, model.CreatedAt);
                    Assert.Equal(modifiedTimestamp, model.LastModifiedAt);
                    Assert.Null(model.DeletedAt);
                });

            Assert.Collection(
                context.ModelsWithNoTimestamps,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                });

            Assert.Collection(
                context.ModelsWithNonNullableTimestamps,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Equal(createdTimestamp, model.CreatedAt);
                    Assert.Equal(modifiedTimestamp, model.LastModifiedAt);
                    Assert.Equal(default, model.DeletedAt);
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
                context.ModelsWithDeletedAtTimestampOnly,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Null(model.DeletedAt);
                });

            Assert.Collection(
                context.ModelsWithLastModifiedAtNonNullableTimestampOnly,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Equal(modifiedTimestamp, model.LastModifiedAt);
                });

            Assert.Collection(
                context.ModelsWithDeletedAtNonNullableTimestampOnly,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Equal(default, model.DeletedAt);
                });

            Assert.Collection(
                context.ModelsWithStringTimestamps,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Null(model.CreatedAt);
                    Assert.Null(model.LastModifiedAt);
                    Assert.Null(model.DeletedAt);
                });
        }

        [Fact]
        public void When_UpdatingChangeTrackingTimestampsOnDeletedEntities_Expect_TimestampsUpdated()
        {
            // Arrange
            using TestContext context = new TestContext(this.dbContextOptions);

            context.ModelsWithAllTimestamps.Add(new Mock.ModelWithAllTimestamps());
            context.ModelsWithNoTimestamps.Add(new Mock.ModelWithNoTimestamps());
            context.ModelsWithNonNullableTimestamps.Add(new Mock.ModelWithNonNullableTimestamps());

            context.ModelsWithCreatedAtTimestampOnly.Add(new Mock.ModelWithCreatedAtTimestampOnly());
            context.ModelsWithLastModifiedAtTimestampOnly.Add(new Mock.ModelWithLastModifiedAtTimestampOnly());
            context.ModelsWithDeletedAtTimestampOnly.Add(new Mock.ModelWithDeletedAtTimestampOnly());
            context.ModelsWithLastModifiedAtNonNullableTimestampOnly.Add(new Mock.ModelWithLastModifiedAtNonNullableTimestampOnly());
            context.ModelsWithDeletedAtNonNullableTimestampOnly.Add(new Mock.ModelWithDeletedAtNonNullableTimestampOnly());

            context.ModelsWithStringTimestamps.Add(new Mock.ModelWithStringTimestamps());

            DateTime createdTimestamp = DateTime.UtcNow;
            DateTime deletedTimestamp = createdTimestamp.AddMinutes(1);

            context.ChangeTracker.UpdateChangeTrackingTimestamps(createdTimestamp);
            context.SaveChanges();

            foreach (EntityEntry entity in context.ChangeTracker.Entries())
            {
                entity.State = EntityState.Deleted;
            }

            // Act
            context.ChangeTracker.UpdateChangeTrackingTimestamps(deletedTimestamp);
            context.SaveChanges();

            // Assert
            Assert.Collection(
                context.ModelsWithAllTimestamps,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Equal(createdTimestamp, model.CreatedAt);
                    Assert.Null(model.LastModifiedAt);
                    Assert.Equal(deletedTimestamp, model.DeletedAt);
                });

            Assert.Empty(context.ModelsWithNoTimestamps);

            Assert.Collection(
                context.ModelsWithNonNullableTimestamps,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Equal(createdTimestamp, model.CreatedAt);
                    Assert.Equal(createdTimestamp, model.LastModifiedAt);
                    Assert.Equal(deletedTimestamp, model.DeletedAt);
                });

            Assert.Empty(context.ModelsWithCreatedAtTimestampOnly);
            Assert.Empty(context.ModelsWithLastModifiedAtTimestampOnly);

            Assert.Collection(
                context.ModelsWithDeletedAtTimestampOnly,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Equal(deletedTimestamp, model.DeletedAt);
                });

            Assert.Empty(context.ModelsWithLastModifiedAtNonNullableTimestampOnly);

            Assert.Collection(
                context.ModelsWithDeletedAtNonNullableTimestampOnly,
                model =>
                {
                    Assert.NotEqual(default, model.Id);
                    Assert.Equal(deletedTimestamp, model.DeletedAt);
                });

            Assert.Empty(context.ModelsWithStringTimestamps);
        }
    }

    private class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        {
        }

        public DbSet<Mock.ModelWithAllTimestamps> ModelsWithAllTimestamps => this.Set<Mock.ModelWithAllTimestamps>();

        public DbSet<Mock.ModelWithNoTimestamps> ModelsWithNoTimestamps => this.Set<Mock.ModelWithNoTimestamps>();

        public DbSet<Mock.ModelWithNonNullableTimestamps> ModelsWithNonNullableTimestamps => this.Set<Mock.ModelWithNonNullableTimestamps>();

        public DbSet<Mock.ModelWithCreatedAtTimestampOnly> ModelsWithCreatedAtTimestampOnly => this.Set<Mock.ModelWithCreatedAtTimestampOnly>();

        public DbSet<Mock.ModelWithLastModifiedAtTimestampOnly> ModelsWithLastModifiedAtTimestampOnly => this.Set<Mock.ModelWithLastModifiedAtTimestampOnly>();

        public DbSet<Mock.ModelWithDeletedAtTimestampOnly> ModelsWithDeletedAtTimestampOnly => this.Set<Mock.ModelWithDeletedAtTimestampOnly>();

        public DbSet<Mock.ModelWithLastModifiedAtNonNullableTimestampOnly> ModelsWithLastModifiedAtNonNullableTimestampOnly => this.Set<Mock.ModelWithLastModifiedAtNonNullableTimestampOnly>();

        public DbSet<Mock.ModelWithDeletedAtNonNullableTimestampOnly> ModelsWithDeletedAtNonNullableTimestampOnly => this.Set<Mock.ModelWithDeletedAtNonNullableTimestampOnly>();

        public DbSet<Mock.ModelWithStringTimestamps> ModelsWithStringTimestamps => this.Set<Mock.ModelWithStringTimestamps>();
    }
}
