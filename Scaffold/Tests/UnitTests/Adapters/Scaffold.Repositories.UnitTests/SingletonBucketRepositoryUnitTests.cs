namespace Scaffold.Repositories.UnitTests;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Domain.Aggregates.Bucket;
using Xunit;

public class SingletonBucketRepositoryUnitTests
{
    private static ServiceProvider CreateNewServiceProvider()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDbContextFactory<BucketContext>(builder => builder.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        return services.BuildServiceProvider();
    }

    public class AddBucket : SingletonBucketRepositoryUnitTests
    {
        [Fact]
        public void When_AddingBucket_Expect_Saved()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketRepository repository = new SingletonBucketRepository(contextFactory);

            Bucket bucket = new Bucket();

            // Act
            repository.Add(bucket);

            // Assert
            using BucketContext context = contextFactory.CreateDbContext();

            Assert.Single(context.Buckets);

            Bucket result = context.Buckets.Find(bucket.Id);
            Assert.NotEqual(bucket, result);
            Assert.Equal(bucket.Id, result.Id);
        }
    }

    public class AddBuckets : SingletonBucketRepositoryUnitTests
    {
        [Fact]
        public void When_AddingEnumerableOfBuckets_Expect_Saved()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketRepository repository = new SingletonBucketRepository(contextFactory);

            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
            };

            // Act
            repository.Add(buckets);

            // Assert
            using BucketContext context = contextFactory.CreateDbContext();

            Assert.Collection(
                context.Buckets,
                bucket => Assert.Equal("Bucket 1", bucket.Name),
                bucket => Assert.Equal("Bucket 2", bucket.Name),
                bucket => Assert.Equal("Bucket 3", bucket.Name));
        }

        [Fact]
        public void When_AddingEmptyEnumerableOfBuckets_Expect_NoChange()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketRepository repository = new SingletonBucketRepository(contextFactory);

            // Act
            Exception exception = Record.Exception(() => repository.Add(Array.Empty<Bucket>()));

            // Assert
            Assert.Null(exception);

            using BucketContext context = contextFactory.CreateDbContext();
            Assert.Empty(context.Buckets);
        }
    }

    public class AddBucketAsync : SingletonBucketRepositoryUnitTests
    {
        [Fact]
        public async Task When_AddingBucket_Expect_Saved()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketRepository repository = new SingletonBucketRepository(contextFactory);

            Bucket bucket = new Bucket();

            // Act
            await repository.AddAsync(bucket);

            // Assert
            using BucketContext context = contextFactory.CreateDbContext();

            Assert.Single(context.Buckets);

            Bucket result = context.Buckets.Find(bucket.Id);
            Assert.NotEqual(bucket, result);
            Assert.Equal(bucket.Id, result.Id);
        }
    }

    public class AddBucketsAsync : SingletonBucketRepositoryUnitTests
    {
        [Fact]
        public async Task When_AddingEnumerableOfBuckets_Expect_Saved()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketRepository repository = new SingletonBucketRepository(contextFactory);

            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
            };

            // Act
            await repository.AddAsync(buckets);

            // Assert
            using BucketContext context = contextFactory.CreateDbContext();

            Assert.Collection(
                context.Buckets,
                bucket => Assert.Equal("Bucket 1", bucket.Name),
                bucket => Assert.Equal("Bucket 2", bucket.Name),
                bucket => Assert.Equal("Bucket 3", bucket.Name));
        }

        [Fact]
        public async Task When_AddingEmptyEnumerableOfBuckets_Expect_NoChange()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketRepository repository = new SingletonBucketRepository(contextFactory);

            // Act
            Exception exception = await Record.ExceptionAsync(() => repository.AddAsync(Array.Empty<Bucket>()));

            // Assert
            Assert.Null(exception);

            using BucketContext context = contextFactory.CreateDbContext();
            Assert.Empty(context.Buckets);
        }
    }

    public class RemoveBucket : SingletonBucketRepositoryUnitTests
    {
        [Fact]
        public void When_RemovingExistingBucket_Expect_Removed()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketRepository repository = new SingletonBucketRepository(contextFactory);

            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
                new Bucket { Name = "Bucket 4" },
                new Bucket { Name = "Bucket 5" },
            };

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(buckets);
                context.SaveChanges();
            }

            // Act
            repository.Remove(buckets[2]);

            // Assert
            using (BucketContext context = contextFactory.CreateDbContext())
            {
                Assert.Null(context.Buckets.Find(buckets[2].Id));

                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 2", bucket.Name),
                    bucket => Assert.Equal("Bucket 4", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));
            }
        }
    }

    public class RemoveBuckets : SingletonBucketRepositoryUnitTests
    {
        [Fact]
        public void When_RemovingEnumerableOfBuckets_Expect_Removed()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketRepository repository = new SingletonBucketRepository(contextFactory);

            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
                new Bucket { Name = "Bucket 4" },
                new Bucket { Name = "Bucket 5" },
            };

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(buckets);
                context.SaveChanges();
            }

            // Act
            repository.Remove(new Bucket[] { buckets[1], buckets[3] });

            // Assert
            using (BucketContext context = contextFactory.CreateDbContext())
            {
                Assert.Null(context.Buckets.Find(buckets[1].Id));
                Assert.Null(context.Buckets.Find(buckets[3].Id));

                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));
            }
        }

        [Fact]
        public void When_RemovingEmptyEnumerableOfBuckets_Expect_NoChange()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketRepository repository = new SingletonBucketRepository(contextFactory);

            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
                new Bucket { Name = "Bucket 4" },
                new Bucket { Name = "Bucket 5" },
            };

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(buckets);
                context.SaveChanges();
            }

            // Act
            Exception exception = Record.Exception(() => repository.Remove(Array.Empty<Bucket>()));

            // Assert
            Assert.Null(exception);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 2", bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name),
                    bucket => Assert.Equal("Bucket 4", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));
            }
        }
    }

    public class RemoveBucketAsync : SingletonBucketRepositoryUnitTests
    {
        [Fact]
        public async Task When_RemovingExistingBucket_Expect_Removed()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketRepository repository = new SingletonBucketRepository(contextFactory);

            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
                new Bucket { Name = "Bucket 4" },
                new Bucket { Name = "Bucket 5" },
            };

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(buckets);
                context.SaveChanges();
            }

            // Act
            await repository.RemoveAsync(buckets[2]);

            // Assert
            using (BucketContext context = contextFactory.CreateDbContext())
            {
                Assert.Null(context.Buckets.Find(buckets[2].Id));

                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 2", bucket.Name),
                    bucket => Assert.Equal("Bucket 4", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));
            }
        }
    }

    public class RemoveBucketsAsync : SingletonBucketRepositoryUnitTests
    {
        [Fact]
        public async Task When_RemovingEnumerableOfBuckets_Expect_Removed()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketRepository repository = new SingletonBucketRepository(contextFactory);

            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
                new Bucket { Name = "Bucket 4" },
                new Bucket { Name = "Bucket 5" },
            };

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(buckets);
                context.SaveChanges();
            }

            // Act
            await repository.RemoveAsync(new Bucket[] { buckets[1], buckets[3] });

            // Assert
            using (BucketContext context = contextFactory.CreateDbContext())
            {
                Assert.Null(context.Buckets.Find(buckets[1].Id));
                Assert.Null(context.Buckets.Find(buckets[3].Id));

                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));
            }
        }

        [Fact]
        public async Task When_RemovingEmptyEnumerableOfBuckets_Expect_NoChange()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketRepository repository = new SingletonBucketRepository(contextFactory);

            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
                new Bucket { Name = "Bucket 4" },
                new Bucket { Name = "Bucket 5" },
            };

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(buckets);
                context.SaveChanges();
            }

            // Act
            Exception exception = await Record.ExceptionAsync(() => repository.RemoveAsync(Array.Empty<Bucket>()));

            // Assert
            Assert.Null(exception);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 2", bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name),
                    bucket => Assert.Equal("Bucket 4", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));
            }
        }
    }

    public class UpdateBucket : SingletonBucketRepositoryUnitTests
    {
    }

    public class UpdateBuckets : SingletonBucketRepositoryUnitTests
    {
    }

    public class UpdateBucketAsync : SingletonBucketRepositoryUnitTests
    {
    }

    public class UpdateBucketsAsync : SingletonBucketRepositoryUnitTests
    {
    }
}
