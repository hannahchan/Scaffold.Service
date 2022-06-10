namespace Scaffold.Repositories.UnitTests;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Application.Common.Models;
using Scaffold.Domain.Aggregates.Bucket;
using Xunit;

public class SingletonBucketReadRepositoryUnitTests
{
    private readonly Bucket[] testBuckets =
    {
        new Bucket { Name = "B", Description = "1", Size = 3 },
        new Bucket { Name = "A", Description = "3", Size = 9 },
        new Bucket { Name = "B", Description = "1", Size = 7 },
        new Bucket { Name = "B", Description = "3", Size = 10 },
        new Bucket { Name = "B", Description = "3", Size = 4 },
        new Bucket { Name = "A", Description = "3", Size = 6 },
        new Bucket { Name = "B", Description = "2", Size = 1 },
        new Bucket { Name = "A", Description = "2", Size = 11 },
        new Bucket { Name = "A", Description = "1", Size = 5 },
        new Bucket { Name = "A", Description = "2", Size = 12 },
        new Bucket { Name = "B", Description = "2", Size = 2 },
        new Bucket { Name = "A", Description = "1", Size = 8 },
    };

    private static ServiceProvider CreateNewServiceProvider()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDbContextFactory<BucketContext>(builder => builder.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        return services.BuildServiceProvider();
    }

    public class GetWithId : SingletonBucketReadRepositoryUnitTests
    {
        [Fact]
        public void When_GettingExistingBucket_Expect_ExistingBucket()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            Bucket bucket = new Bucket();

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.Add(bucket);
                context.SaveChanges();
            }

            // Act
            Bucket result = repository.Get(bucket.Id);

            // Assert
            Assert.NotEqual(bucket, result);
            Assert.Equal(bucket.Id, result.Id);
            Assert.Equal(bucket.Name, result.Name);
        }

        [Fact]
        public void When_GettingNonExistingBucket_Expect_Null()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            // Act
            Bucket result = repository.Get(new Random().Next());

            // Assert
            Assert.Null(result);
        }
    }

    public class GetWithPredicate : SingletonBucketReadRepositoryUnitTests
    {
        [Fact]
        public void When_GettingBucketsWithPredicate_Expect_AllBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal("Bucket 1", bucket.Name),
                bucket => Assert.Equal("Bucket 2", bucket.Name),
                bucket => Assert.Equal("Bucket 3", bucket.Name));
        }

        [Fact]
        public void When_GettingBucketsWithPredicate_Expect_Empty()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => false);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void When_GettingBucketsWithPredicate_Expect_SomeBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Size = 1 },
                    new Bucket { Size = 2 },
                    new Bucket { Size = 3 },
                    new Bucket { Size = 4 },
                    new Bucket { Size = 5 },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => bucket.Size == 2 || bucket.Size == 5);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal(2, bucket.Size),
                bucket => Assert.Equal(5, bucket.Size));
        }

        [Fact]
        public void When_GettingBucketsWithNoLimit_Expect_AllBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, null);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal("Bucket 1", bucket.Name),
                bucket => Assert.Equal("Bucket 2", bucket.Name),
                bucket => Assert.Equal("Bucket 3", bucket.Name));
        }

        [Fact]
        public void When_GettingBucketsWithLimit_Expect_LimitedBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, 2);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal("Bucket 1", bucket.Name),
                bucket => Assert.Equal("Bucket 2", bucket.Name));
        }

        [Fact]
        public void When_GettingBucketsWithNoOffset_Expect_AllBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, null, null);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal("Bucket 1", bucket.Name),
                bucket => Assert.Equal("Bucket 2", bucket.Name),
                bucket => Assert.Equal("Bucket 3", bucket.Name));
        }

        [Fact]
        public void When_GettingBucketsWithOffset_Expect_OffsetBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, null, 1);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal("Bucket 2", bucket.Name),
                bucket => Assert.Equal("Bucket 3", bucket.Name));
        }

        [Fact]
        public void When_GettingBucketsWithLimitAndOffset_Expect_LimitedAndOffsetBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, 1, 1);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal("Bucket 2", bucket.Name));
        }
    }

    public class GetWithIdAsync : SingletonBucketReadRepositoryUnitTests
    {
        [Fact]
        public async Task When_GettingExistingBucket_Expect_ExistingBucket()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            Bucket bucket = new Bucket();

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.Add(bucket);
                context.SaveChanges();
            }

            // Act
            Bucket result = await repository.GetAsync(bucket.Id);

            // Assert
            Assert.NotEqual(bucket, result);
            Assert.Equal(bucket.Id, result.Id);
            Assert.Equal(bucket.Name, result.Name);
        }

        [Fact]
        public async Task When_GettingNonExistingBucket_Expect_Null()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            // Act
            Bucket result = await repository.GetAsync(new Random().Next());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task When_GettingBucketAndCancellationIsRequested_Expect_OperationCanceledException()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            // Act
            Exception exception = await Record.ExceptionAsync(() => repository.GetAsync(new Random().Next(), new CancellationToken(true)));

            // Assert
            Assert.IsType<OperationCanceledException>(exception);
        }
    }

    public class GetWithPredicateAsync : SingletonBucketReadRepositoryUnitTests
    {
        [Fact]
        public async Task When_GettingBucketsWithPredicate_Expect_AllBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal("Bucket 1", bucket.Name),
                bucket => Assert.Equal("Bucket 2", bucket.Name),
                bucket => Assert.Equal("Bucket 3", bucket.Name));
        }

        [Fact]
        public async Task When_GettingBucketsWithPredicate_Expect_Empty()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => false);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task When_GettingBucketsWithPredicate_Expect_SomeBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Size = 1 },
                    new Bucket { Size = 2 },
                    new Bucket { Size = 3 },
                    new Bucket { Size = 4 },
                    new Bucket { Size = 5 },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => bucket.Size == 2 || bucket.Size == 5);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal(2, bucket.Size),
                bucket => Assert.Equal(5, bucket.Size));
        }

        [Fact]
        public async Task When_GettingBucketsWithNoLimit_Expect_AllBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, null);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal("Bucket 1", bucket.Name),
                bucket => Assert.Equal("Bucket 2", bucket.Name),
                bucket => Assert.Equal("Bucket 3", bucket.Name));
        }

        [Fact]
        public async Task When_GettingBucketsWithLimit_Expect_LimitedBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, 2);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal("Bucket 1", bucket.Name),
                bucket => Assert.Equal("Bucket 2", bucket.Name));
        }

        [Fact]
        public async Task When_GettingBucketsWithNoOffset_Expect_AllBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, null, null);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal("Bucket 1", bucket.Name),
                bucket => Assert.Equal("Bucket 2", bucket.Name),
                bucket => Assert.Equal("Bucket 3", bucket.Name));
        }

        [Fact]
        public async Task When_GettingBucketsWithOffset_Expect_OffsetBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, null, 1);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal("Bucket 2", bucket.Name),
                bucket => Assert.Equal("Bucket 3", bucket.Name));
        }

        [Fact]
        public async Task When_GettingBucketsWithLimitAndOffset_Expect_LimitedAndOffsetBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                context.SaveChanges();
            }

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, 1, 1);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal("Bucket 2", bucket.Name));
        }

        [Fact]
        public async Task When_GettingBucketsAndCancellationIsRequested_Expect_OperationCanceledException()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            // Act
            Exception exception = await Record.ExceptionAsync(() => repository.GetAsync(
                predicate: bucket => true,
                cancellationToken: new CancellationToken(true)));

            // Assert
            Assert.IsType<OperationCanceledException>(exception);
        }
    }

    public class GetWithSortOrder : SingletonBucketReadRepositoryUnitTests
    {
        [Fact]
        public void When_GettingBucketsOrderedBySizeAscending_Expect_OrderedBySizeAscending()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Size);

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, null, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal(1, bucket.Size),
                bucket => Assert.Equal(2, bucket.Size),
                bucket => Assert.Equal(3, bucket.Size),
                bucket => Assert.Equal(4, bucket.Size),
                bucket => Assert.Equal(5, bucket.Size),
                bucket => Assert.Equal(6, bucket.Size),
                bucket => Assert.Equal(7, bucket.Size),
                bucket => Assert.Equal(8, bucket.Size),
                bucket => Assert.Equal(9, bucket.Size),
                bucket => Assert.Equal(10, bucket.Size),
                bucket => Assert.Equal(11, bucket.Size),
                bucket => Assert.Equal(12, bucket.Size));
        }

        [Fact]
        public void When_GettingBucketsOrderedBySizeDescending_Expect_OrderedBySizeDescending()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderByDescending(bucket => bucket.Size);

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, null, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal(12, bucket.Size),
                bucket => Assert.Equal(11, bucket.Size),
                bucket => Assert.Equal(10, bucket.Size),
                bucket => Assert.Equal(9, bucket.Size),
                bucket => Assert.Equal(8, bucket.Size),
                bucket => Assert.Equal(7, bucket.Size),
                bucket => Assert.Equal(6, bucket.Size),
                bucket => Assert.Equal(5, bucket.Size),
                bucket => Assert.Equal(4, bucket.Size),
                bucket => Assert.Equal(3, bucket.Size),
                bucket => Assert.Equal(2, bucket.Size),
                bucket => Assert.Equal(1, bucket.Size));
        }

        [Fact]
        public void When_GettingBucketsOrderedBySizeWithLimit_Expect_OrderedLimitedBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Size);

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, 6, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal(1, bucket.Size),
                bucket => Assert.Equal(2, bucket.Size),
                bucket => Assert.Equal(3, bucket.Size),
                bucket => Assert.Equal(4, bucket.Size),
                bucket => Assert.Equal(5, bucket.Size),
                bucket => Assert.Equal(6, bucket.Size));
        }

        [Fact]
        public void When_GettingBucketsOrderedBySizeWithOffset_Expect_OrderedOffsetBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Size);

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, null, 6, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal(7, bucket.Size),
                bucket => Assert.Equal(8, bucket.Size),
                bucket => Assert.Equal(9, bucket.Size),
                bucket => Assert.Equal(10, bucket.Size),
                bucket => Assert.Equal(11, bucket.Size),
                bucket => Assert.Equal(12, bucket.Size));
        }

        [Fact]
        public void When_GettingBucketsOrderedBySizeWithLimtAndOffset_Expect_OrderedLimitedAndOffsetBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Size);

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, 6, 3, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal(4, bucket.Size),
                bucket => Assert.Equal(5, bucket.Size),
                bucket => Assert.Equal(6, bucket.Size),
                bucket => Assert.Equal(7, bucket.Size),
                bucket => Assert.Equal(8, bucket.Size),
                bucket => Assert.Equal(9, bucket.Size));
        }

        [Fact]
        public void When_GettingBucketsOrderedByNameAscendingThenByDescriptionAscending_Expect_OrderedByNameAscendingThenByDescriptionAscending()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Name)
                .ThenBy(bucket => bucket.Description);

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, null, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                });
        }

        [Fact]
        public void When_GettingBucketsOrderedByNameDescendingThenByDescriptionDescending_Expect_OrderedByNameDescendingThenByDescriptionDescending()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderByDescending(bucket => bucket.Name)
                .ThenByDescending(bucket => bucket.Description);

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, null, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                });
        }

        [Fact]
        public void When_GettingBucketsOrderedByNameAscendingThenByDescriptionDescending_Expect_OrderedByNameAscendingThenByDescriptionDescending()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Name)
                .ThenByDescending(bucket => bucket.Description);

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, null, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                });
        }

        [Fact]
        public void When_GettingBucketsOrderedByNameDescendingThenByDescriptionAscending_Expect_OrderedByNameDescendingThenByDescriptionAscending()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderByDescending(bucket => bucket.Name)
                .ThenBy(bucket => bucket.Description);

            // Act
            IEnumerable<Bucket> result = repository.Get(bucket => true, null, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                });
        }
    }

    public class GetWithSortOrderAsync : SingletonBucketReadRepositoryUnitTests
    {
        [Fact]
        public async Task When_GettingBucketsOrderedBySizeAscending_Expect_OrderedBySizeAscending()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Size);

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, null, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal(1, bucket.Size),
                bucket => Assert.Equal(2, bucket.Size),
                bucket => Assert.Equal(3, bucket.Size),
                bucket => Assert.Equal(4, bucket.Size),
                bucket => Assert.Equal(5, bucket.Size),
                bucket => Assert.Equal(6, bucket.Size),
                bucket => Assert.Equal(7, bucket.Size),
                bucket => Assert.Equal(8, bucket.Size),
                bucket => Assert.Equal(9, bucket.Size),
                bucket => Assert.Equal(10, bucket.Size),
                bucket => Assert.Equal(11, bucket.Size),
                bucket => Assert.Equal(12, bucket.Size));
        }

        [Fact]
        public async Task When_GettingBucketsOrderedBySizeDescending_Expect_OrderedBySizeDescending()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderByDescending(bucket => bucket.Size);

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, null, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal(12, bucket.Size),
                bucket => Assert.Equal(11, bucket.Size),
                bucket => Assert.Equal(10, bucket.Size),
                bucket => Assert.Equal(9, bucket.Size),
                bucket => Assert.Equal(8, bucket.Size),
                bucket => Assert.Equal(7, bucket.Size),
                bucket => Assert.Equal(6, bucket.Size),
                bucket => Assert.Equal(5, bucket.Size),
                bucket => Assert.Equal(4, bucket.Size),
                bucket => Assert.Equal(3, bucket.Size),
                bucket => Assert.Equal(2, bucket.Size),
                bucket => Assert.Equal(1, bucket.Size));
        }

        [Fact]
        public async Task When_GettingBucketsOrderedBySizeWithLimit_Expect_OrderedLimitedBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Size);

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, 6, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal(1, bucket.Size),
                bucket => Assert.Equal(2, bucket.Size),
                bucket => Assert.Equal(3, bucket.Size),
                bucket => Assert.Equal(4, bucket.Size),
                bucket => Assert.Equal(5, bucket.Size),
                bucket => Assert.Equal(6, bucket.Size));
        }

        [Fact]
        public async Task When_GettingBucketsOrderedBySizeWithOffset_Expect_OrderedOffsetBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Size);

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, null, 6, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal(7, bucket.Size),
                bucket => Assert.Equal(8, bucket.Size),
                bucket => Assert.Equal(9, bucket.Size),
                bucket => Assert.Equal(10, bucket.Size),
                bucket => Assert.Equal(11, bucket.Size),
                bucket => Assert.Equal(12, bucket.Size));
        }

        [Fact]
        public async Task When_GettingBucketsOrderedBySizeWithLimtAndOffset_Expect_OrderedLimitedAndOffsetBuckets()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Size);

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, 6, 3, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket => Assert.Equal(4, bucket.Size),
                bucket => Assert.Equal(5, bucket.Size),
                bucket => Assert.Equal(6, bucket.Size),
                bucket => Assert.Equal(7, bucket.Size),
                bucket => Assert.Equal(8, bucket.Size),
                bucket => Assert.Equal(9, bucket.Size));
        }

        [Fact]
        public async Task When_GettingBucketsOrderedByNameAscendingThenByDescriptionAscending_Expect_OrderedByNameAscendingThenByDescriptionAscending()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Name)
                .ThenBy(bucket => bucket.Description);

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, null, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                });
        }

        [Fact]
        public async Task When_GettingBucketsOrderedByNameDescendingThenByDescriptionDescending_Expect_OrderedByNameDescendingThenByDescriptionDescending()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderByDescending(bucket => bucket.Name)
                .ThenByDescending(bucket => bucket.Description);

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, null, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                });
        }

        [Fact]
        public async Task When_GettingBucketsOrderedByNameAscendingThenByDescriptionDescending_Expect_OrderedByNameAscendingThenByDescriptionDescending()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Name)
                .ThenByDescending(bucket => bucket.Description);

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, null, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                });
        }

        [Fact]
        public async Task When_GettingBucketsOrderedByNameDescendingThenByDescriptionAscending_Expect_OrderedByNameDescendingThenByDescriptionAscending()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketReadRepository repository = new SingletonBucketReadRepository(contextFactory);

            using (BucketContext context = contextFactory.CreateDbContext())
            {
                context.Buckets.AddRange(this.testBuckets);
                context.SaveChanges();
            }

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderByDescending(bucket => bucket.Name)
                .ThenBy(bucket => bucket.Description);

            // Act
            IEnumerable<Bucket> result = await repository.GetAsync(bucket => true, null, null, sortOrder);

            // Assert
            Assert.Collection(
                result,
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("B", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("1", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("2", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                },
                bucket =>
                {
                    Assert.Equal("A", bucket.Name);
                    Assert.Equal("3", bucket.Description);
                });
        }
    }
}
