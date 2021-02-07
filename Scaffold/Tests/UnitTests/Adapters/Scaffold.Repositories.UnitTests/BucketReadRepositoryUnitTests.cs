namespace Scaffold.Repositories.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Common.Models;
    using Scaffold.Domain.Aggregates.Bucket;
    using Xunit;

    public class BucketReadRepositoryUnitTests
    {
        private readonly DbContextOptions<BucketContext.ReadOnly> dbContextOptions;

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

        public BucketReadRepositoryUnitTests()
        {
            this.dbContextOptions = new DbContextOptionsBuilder<BucketContext.ReadOnly>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        public class GetWithId : BucketReadRepositoryUnitTests
        {
            [Fact]
            public void When_GettingExistingBucket_Expect_ExistingBucket()
            {
                // Arrange
                Bucket bucket = new Bucket();

                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.Add(bucket);
                    context.SaveChanges();
                }

                Bucket result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket.Id);
                }

                // Assert
                Assert.NotEqual(bucket, result);
                Assert.Equal(bucket.Id, result.Id);
                Assert.Equal(bucket.Name, result.Name);
            }

            [Fact]
            public void When_GettingNonExistingBucket_Expect_Null()
            {
                // Arrange
                Bucket result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(new Random().Next());
                }

                // Assert
                Assert.Null(result);
            }
        }

        public class GetWithPredicate : BucketReadRepositoryUnitTests
        {
            [Fact]
            public void When_GettingBucketsWithPredicate_Expect_AllBuckets()
            {
                // Arrange
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    context.SaveChanges();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    context.SaveChanges();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => false);
                }

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
            }

            [Fact]
            public void When_GettingBucketsWithPredicate_Expect_SomeBuckets()
            {
                // Arrange
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
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

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => bucket.Size == 2 || bucket.Size == 5);
                }

                // Assert
                Assert.Collection(
                    result,
                    bucket => Assert.Equal(2, bucket.Size),
                    bucket => Assert.Equal(5, bucket.Size));
            }

            [Fact]
            public void When_GettingBucketsWithNullPredicate_Expect_ArgumentNullException()
            {
                // Arrange
                Exception exception;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    exception = Record.Exception(() => repository.Get(null));
                }

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("predicate", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketReadRepository).Assembly.GetName().Name, exception.Source);
            }

            [Fact]
            public void When_GettingBucketsWithNoLimit_Expect_AllBuckets()
            {
                // Arrange
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    context.SaveChanges();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, null);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    context.SaveChanges();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, 2);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    context.SaveChanges();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, null, null);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    context.SaveChanges();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, null, 1);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    context.SaveChanges();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, 1, 1);
                }

                // Assert
                Assert.Collection(
                    result,
                    bucket => Assert.Equal("Bucket 2", bucket.Name));
            }
        }

        public class GetWithIdAsync : BucketReadRepositoryUnitTests
        {
            [Fact]
            public async Task When_GettingExistingBucket_Expect_ExistingBucket()
            {
                // Arrange
                Bucket bucket = new Bucket();

                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.Add(bucket);
                    await context.SaveChangesAsync();
                }

                Bucket result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket.Id);
                }

                // Assert
                Assert.NotEqual(bucket, result);
                Assert.Equal(bucket.Id, result.Id);
                Assert.Equal(bucket.Name, result.Name);
            }

            [Fact]
            public async Task When_GettingNonExistingBucket_Expect_Null()
            {
                // Arrange
                Bucket result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(new Random().Next());
                }

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public async Task When_GettingBucketAndCancellationIsRequested_Expect_OperationCanceledException()
            {
                // Arrange
                Exception exception;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    exception = await Record.ExceptionAsync(() => repository.GetAsync(new Random().Next(), new CancellationToken(true)));
                }

                // Assert
                Assert.IsType<OperationCanceledException>(exception);
            }
        }

        public class GetWithPredicateAsync : BucketReadRepositoryUnitTests
        {
            [Fact]
            public async Task When_GettingBucketsWithPredicate_Expect_AllBuckets()
            {
                // Arrange
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    await context.SaveChangesAsync();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    await context.SaveChangesAsync();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => false);
                }

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
            }

            [Fact]
            public async Task When_GettingBucketsWithPredicate_Expect_SomeBuckets()
            {
                // Arrange
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Size = 1 },
                        new Bucket { Size = 2 },
                        new Bucket { Size = 3 },
                        new Bucket { Size = 4 },
                        new Bucket { Size = 5 },
                    });

                    await context.SaveChangesAsync();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => bucket.Size == 2 || bucket.Size == 5);
                }

                // Assert
                Assert.Collection(
                    result,
                    bucket => Assert.Equal(2, bucket.Size),
                    bucket => Assert.Equal(5, bucket.Size));
            }

            [Fact]
            public async Task When_GettingBucketsWithNullPredicate_Expect_ArgumentNullException()
            {
                // Arrange
                Exception exception;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    exception = await Record.ExceptionAsync(() => repository.GetAsync(null));
                }

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("predicate", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketReadRepository).Assembly.GetName().Name, exception.Source);
            }

            [Fact]
            public async Task When_GettingBucketsWithNoLimit_Expect_AllBuckets()
            {
                // Arrange
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    await context.SaveChangesAsync();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, null);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    await context.SaveChangesAsync();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, 2);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    await context.SaveChangesAsync();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    await context.SaveChangesAsync();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, null, 1);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(new Bucket[]
                    {
                        new Bucket { Name = "Bucket 1" },
                        new Bucket { Name = "Bucket 2" },
                        new Bucket { Name = "Bucket 3" },
                    });

                    await context.SaveChangesAsync();
                }

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, 1, 1);
                }

                // Assert
                Assert.Collection(
                    result,
                    bucket => Assert.Equal("Bucket 2", bucket.Name));
            }

            [Fact]
            public async Task When_GettingBucketsAndCancellationIsRequested_Expect_OperationCanceledException()
            {
                // Arrange
                Exception exception;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    exception = await Record.ExceptionAsync(() => repository.GetAsync(
                        predicate: bucket => true,
                        cancellationToken: new CancellationToken(true)));
                }

                // Assert
                Assert.IsType<OperationCanceledException>(exception);
            }
        }

        public class GetWithSortOrder : BucketReadRepositoryUnitTests
        {
            [Fact]
            public void When_GettingBucketsOrderedBySizeAscending_Expect_OrderedBySizeAscending()
            {
                // Arrange
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Size);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, null, null, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderByDescending(bucket => bucket.Size);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, null, null, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Size);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, 6, null, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Size);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, null, 6, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Size);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, 6, 3, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Name)
                    .ThenBy(bucket => bucket.Description);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, null, null, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderByDescending(bucket => bucket.Name)
                    .ThenByDescending(bucket => bucket.Description);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, null, null, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Name)
                    .ThenByDescending(bucket => bucket.Description);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, null, null, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderByDescending(bucket => bucket.Name)
                    .ThenBy(bucket => bucket.Description);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = repository.Get(bucket => true, null, null, sortOrder);
                }

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

        public class GetWithSortOrderAsync : BucketReadRepositoryUnitTests
        {
            [Fact]
            public async Task When_GettingBucketsOrderedBySizeAscending_Expect_OrderedBySizeAscending()
            {
                // Arrange
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Size);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderByDescending(bucket => bucket.Size);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Size);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, 6, null, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Size);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, null, 6, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Size);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, 6, 3, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Name)
                    .ThenBy(bucket => bucket.Description);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderByDescending(bucket => bucket.Name)
                    .ThenByDescending(bucket => bucket.Description);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Name)
                    .ThenByDescending(bucket => bucket.Description);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null, sortOrder);
                }

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
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    context.Buckets.AddRange(this.testBuckets);
                    context.SaveChanges();
                }

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderByDescending(bucket => bucket.Name)
                    .ThenBy(bucket => bucket.Description);

                IEnumerable<Bucket> result;

                // Act
                using (BucketContext.ReadOnly context = new BucketContext.ReadOnly(this.dbContextOptions))
                {
                    BucketReadRepository repository = new BucketReadRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null, sortOrder);
                }

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
}
