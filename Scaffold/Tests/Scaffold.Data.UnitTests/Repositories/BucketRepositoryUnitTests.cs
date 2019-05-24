namespace Scaffold.Data.UnitTests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Interfaces;
    using Scaffold.Application.Models;
    using Scaffold.Data;
    using Scaffold.Data.Repositories;
    using Scaffold.Domain.Entities;
    using Xunit;

    public class BucketRepositoryUnitTests
    {
        private readonly DbContextOptions<BucketContext> dbContextOptions;

        private IList<Bucket> testBuckets = new List<Bucket>()
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

        public BucketRepositoryUnitTests()
        {
            this.dbContextOptions = new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        public class Add : BucketRepositoryUnitTests
        {
            [Fact]
            public void When_AddingBucket_Expect_Saved()
            {
                // Arrange
                Bucket bucket = new Bucket();

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    repository.Add(bucket);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Bucket result = context.Set<Bucket>().Find(bucket.Id);

                    Assert.Equal(1, context.Set<Bucket>().Count());
                    Assert.NotEqual(bucket, result);
                    Assert.Equal(bucket.Id, result.Id);
                }
            }

            [Fact]
            public void When_AddingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                IBucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = Record.Exception(() => repository.Add(null));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }
        }

        public class AddAsync : BucketRepositoryUnitTests
        {
            [Fact]
            public async Task When_AddingBucket_Expect_Saved()
            {
                // Arrange
                Bucket bucket = new Bucket();

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    await repository.AddAsync(bucket);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Bucket result = context.Set<Bucket>().Find(bucket.Id);

                    Assert.Equal(1, context.Set<Bucket>().Count());
                    Assert.NotEqual(bucket, result);
                    Assert.Equal(bucket.Id, result.Id);
                }
            }

            [Fact]
            public async Task When_AddingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                IBucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.AddAsync(null));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }
        }

        public class Get : BucketRepositoryUnitTests
        {
            [Fact]
            public void When_GettingExistingBucket_Expect_ExistingBucket()
            {
                // Arrange
                Bucket bucket = new Bucket();

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(bucket);
                    context.SaveChanges();
                }

                Bucket result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
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
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(new Random().Next(int.MaxValue));
                }

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void When_GettingBucketsWithPredicate_Expect_AllBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket());
                    context.Set<Bucket>().Add(new Bucket());
                    context.Set<Bucket>().Add(new Bucket());
                    context.SaveChanges();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(3, result.Count);
            }

            [Fact]
            public void When_GettingBucketsWithPredicate_Expect_EmptyList()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket());
                    context.Set<Bucket>().Add(new Bucket());
                    context.Set<Bucket>().Add(new Bucket());
                    context.SaveChanges();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
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
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket { Size = 1 });
                    context.Set<Bucket>().Add(new Bucket { Size = 2 });
                    context.Set<Bucket>().Add(new Bucket { Size = 3 });
                    context.Set<Bucket>().Add(new Bucket { Size = 5 });
                    context.Set<Bucket>().Add(new Bucket { Size = 8 });
                    context.SaveChanges();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => bucket.Size == 2 || bucket.Size == 5);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(2, result.Count);
            }

            [Fact]
            public void When_GettingBucketsWithNullPredicate_Expect_ArgumentNullException()
            {
                // Arrange
                Exception exception;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    exception = Record.Exception(() => repository.Get(null));
                }

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }

            [Fact]
            public void When_GettingBucketsWithNoLimit_Expect_AllBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 1" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 2" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 3" });
                    context.SaveChanges();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, null);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(3, result.Count);
                Assert.Equal("Bucket 1", result[0].Name);
                Assert.Equal("Bucket 2", result[1].Name);
                Assert.Equal("Bucket 3", result[2].Name);
            }

            [Fact]
            public void When_GettingBucketsWithLimit_Expect_LimitedBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 1" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 2" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 3" });
                    context.SaveChanges();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, 2);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(2, result.Count);
                Assert.Equal("Bucket 1", result[0].Name);
                Assert.Equal("Bucket 2", result[1].Name);
            }

            [Fact]
            public void When_GettingBucketsWithNoOffset_Expect_AllBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 1" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 2" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 3" });
                    context.SaveChanges();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, null, null);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(3, result.Count);
                Assert.Equal("Bucket 1", result[0].Name);
                Assert.Equal("Bucket 2", result[1].Name);
                Assert.Equal("Bucket 3", result[2].Name);
            }

            [Fact]
            public void When_GettingBucketsWithOffset_Expect_OffsetBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 1" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 2" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 3" });
                    context.SaveChanges();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, null, 1);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(2, result.Count);
                Assert.Equal("Bucket 2", result[0].Name);
                Assert.Equal("Bucket 3", result[1].Name);
            }

            [Fact]
            public void When_GettingBucketsWithLimitAndOffset_Expect_LimitedAndOffsetBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 1" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 2" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 3" });
                    context.SaveChanges();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, 1, 1);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(1, result.Count);
                Assert.Equal("Bucket 2", result[0].Name);
            }
        }

        public class GetAsync : BucketRepositoryUnitTests
        {
            [Fact]
            public async Task When_GettingExistingBucket_Expect_ExistingBucket()
            {
                // Arrange
                Bucket bucket = new Bucket();

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(bucket);
                    await context.SaveChangesAsync();
                }

                Bucket result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
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
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(new Random().Next(int.MaxValue));
                }

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public async Task When_GettingBucketsWithPredicate_Expect_AllBuckets()
            {
                // Arrange
                Bucket bucket1 = new Bucket();
                Bucket bucket2 = new Bucket();
                Bucket bucket3 = new Bucket();

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(bucket1);
                    context.Set<Bucket>().Add(bucket2);
                    context.Set<Bucket>().Add(bucket3);
                    await context.SaveChangesAsync();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(3, result.Count);
            }

            [Fact]
            public async Task When_GettingBucketsWithPredicate_Expect_NoBuckets()
            {
                // Arrange
                Bucket bucket1 = new Bucket();
                Bucket bucket2 = new Bucket();
                Bucket bucket3 = new Bucket();

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(bucket1);
                    context.Set<Bucket>().Add(bucket2);
                    context.Set<Bucket>().Add(bucket3);
                    await context.SaveChangesAsync();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
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
                Bucket bucket1 = new Bucket { Size = 1 };
                Bucket bucket2 = new Bucket { Size = 2 };
                Bucket bucket3 = new Bucket { Size = 3 };
                Bucket bucket4 = new Bucket { Size = 5 };
                Bucket bucket5 = new Bucket { Size = 8 };

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(bucket1);
                    context.Set<Bucket>().Add(bucket2);
                    context.Set<Bucket>().Add(bucket3);
                    context.Set<Bucket>().Add(bucket4);
                    context.Set<Bucket>().Add(bucket5);
                    await context.SaveChangesAsync();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => bucket.Size == 2 || bucket.Size == 5);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(2, result.Count);
            }

            [Fact]
            public async Task When_GettingBucketsWithNullPredicate_Expect_ArgumentNullException()
            {
                // Arrange
                Exception exception;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    exception = await Record.ExceptionAsync(() => repository.GetAsync(null));
                }

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }

            [Fact]
            public async Task When_GettingBucketsWithNoLimit_Expect_AllBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 1" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 2" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 3" });
                    await context.SaveChangesAsync();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, null);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(3, result.Count);
                Assert.Equal("Bucket 1", result[0].Name);
                Assert.Equal("Bucket 2", result[1].Name);
                Assert.Equal("Bucket 3", result[2].Name);
            }

            [Fact]
            public async Task When_GettingBucketsWithLimit_Expect_LimitedBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 1" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 2" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 3" });
                    await context.SaveChangesAsync();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, 2);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(2, result.Count);
                Assert.Equal("Bucket 1", result[0].Name);
                Assert.Equal("Bucket 2", result[1].Name);
            }

            [Fact]
            public async Task When_GettingBucketsWithNoOffset_Expect_AllBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 1" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 2" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 3" });
                    await context.SaveChangesAsync();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(3, result.Count);
                Assert.Equal("Bucket 1", result[0].Name);
                Assert.Equal("Bucket 2", result[1].Name);
                Assert.Equal("Bucket 3", result[2].Name);
            }

            [Fact]
            public async Task When_GettingBucketsWithOffset_Expect_OffsetBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 1" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 2" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 3" });
                    await context.SaveChangesAsync();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, null, 1);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(2, result.Count);
                Assert.Equal("Bucket 2", result[0].Name);
                Assert.Equal("Bucket 3", result[1].Name);
            }

            [Fact]
            public async Task When_GettingBucketsWithLimitAndOffset_Expect_LimitedAndOffsetBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 1" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 2" });
                    context.Set<Bucket>().Add(new Bucket { Name = "Bucket 3" });
                    await context.SaveChangesAsync();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, 1, 1);
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(1, result.Count);
                Assert.Equal("Bucket 2", result[0].Name);
            }
        }

        public class GetWithOrdering : BucketRepositoryUnitTests
        {
            [Fact]
            public void When_GettingBucketsOrderedBySizeAscending_Expect_OrderedBySizeAscending()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        context.Set<Bucket>().Add(bucket);
                    }

                    context.SaveChanges();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Size", true));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, null, null, ordering);
                }

                // Assert
                for (int i = 1; i <= 12; i++)
                {
                    Assert.Equal(i, result[i - 1].Size);
                }
            }

            [Fact]
            public void When_GettingBucketsOrderedBySizeDescending_Expect_OrderedBySizeDescending()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        context.Set<Bucket>().Add(bucket);
                    }

                    context.SaveChanges();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Size", false));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, null, null, ordering);
                }

                // Assert
                for (int i = 1; i <= 12; i++)
                {
                    Assert.Equal(12 - (i - 1), result[i - 1].Size);
                }
            }

            [Fact]
            public void When_GettingBucketsOrderedBySizeWithLimit_Expect_OrderedLimitedBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        context.Set<Bucket>().Add(bucket);
                    }

                    context.SaveChanges();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Size", true));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, 6, null, ordering);
                }

                // Assert
                for (int i = 1; i <= 6; i++)
                {
                    Assert.Equal(i, result[i - 1].Size);
                }
            }

            [Fact]
            public void When_GettingBucketsOrderedBySizeWithOffset_Expect_OrderedOffsetBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        context.Set<Bucket>().Add(bucket);
                    }

                    context.SaveChanges();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Size", true));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, null, 6, ordering);
                }

                // Assert
                for (int i = 1; i <= 6; i++)
                {
                    Assert.Equal(i + 6, result[i - 1].Size);
                }
            }

            [Fact]
            public void When_GettingBucketsOrderedBySizeWithLimtAndOffset_Expect_OrderedLimitedAndOffsetBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        context.Set<Bucket>().Add(bucket);
                    }

                    context.SaveChanges();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Size", true));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, 6, 3, ordering);
                }

                // Assert
                for (int i = 1; i <= 6; i++)
                {
                    Assert.Equal(i + 3, result[i - 1].Size);
                }
            }

            [Fact]
            public void When_GettingBucketsOrderedByNameAscendingThenByDescriptionAscending_Expect_OrderedByNameAscendingThenByDescriptionAscending()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        context.Set<Bucket>().Add(bucket);
                    }

                    context.SaveChanges();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Name", true));
                ordering.Add(new OrderBy("Description", true));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, null, null, ordering);
                }

                // Assert
                Assert.Equal("A", result[0].Name);
                Assert.Equal("A", result[1].Name);
                Assert.Equal("A", result[2].Name);
                Assert.Equal("A", result[3].Name);
                Assert.Equal("A", result[4].Name);
                Assert.Equal("A", result[5].Name);
                Assert.Equal("B", result[6].Name);
                Assert.Equal("B", result[7].Name);
                Assert.Equal("B", result[8].Name);
                Assert.Equal("B", result[9].Name);
                Assert.Equal("B", result[10].Name);
                Assert.Equal("B", result[11].Name);

                Assert.Equal("1", result[0].Description);
                Assert.Equal("1", result[1].Description);
                Assert.Equal("2", result[2].Description);
                Assert.Equal("2", result[3].Description);
                Assert.Equal("3", result[4].Description);
                Assert.Equal("3", result[5].Description);
                Assert.Equal("1", result[6].Description);
                Assert.Equal("1", result[7].Description);
                Assert.Equal("2", result[8].Description);
                Assert.Equal("2", result[9].Description);
                Assert.Equal("3", result[10].Description);
                Assert.Equal("3", result[11].Description);
            }

            [Fact]
            public void When_GettingBucketsOrderedByNameDescendingThenByDescriptionDescending_Expect_OrderedByNameDescendingThenByDescriptionDescending()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        context.Set<Bucket>().Add(bucket);
                    }

                    context.SaveChanges();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Name", false));
                ordering.Add(new OrderBy("Description", false));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, null, null, ordering);
                }

                // Assert
                Assert.Equal("B", result[0].Name);
                Assert.Equal("B", result[1].Name);
                Assert.Equal("B", result[2].Name);
                Assert.Equal("B", result[3].Name);
                Assert.Equal("B", result[4].Name);
                Assert.Equal("B", result[5].Name);
                Assert.Equal("A", result[6].Name);
                Assert.Equal("A", result[7].Name);
                Assert.Equal("A", result[8].Name);
                Assert.Equal("A", result[9].Name);
                Assert.Equal("A", result[10].Name);
                Assert.Equal("A", result[11].Name);

                Assert.Equal("3", result[0].Description);
                Assert.Equal("3", result[1].Description);
                Assert.Equal("2", result[2].Description);
                Assert.Equal("2", result[3].Description);
                Assert.Equal("1", result[4].Description);
                Assert.Equal("1", result[5].Description);
                Assert.Equal("3", result[6].Description);
                Assert.Equal("3", result[7].Description);
                Assert.Equal("2", result[8].Description);
                Assert.Equal("2", result[9].Description);
                Assert.Equal("1", result[10].Description);
                Assert.Equal("1", result[11].Description);
            }

            [Fact]
            public void When_GettingBucketsOrderedByNameAscendingThenByDescriptionDescending_Expect_OrderedByNameAscendingThenByDescriptionDescending()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        context.Set<Bucket>().Add(bucket);
                    }

                    context.SaveChanges();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Name", true));
                ordering.Add(new OrderBy("Description", false));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, null, null, ordering);
                }

                // Assert
                Assert.Equal("A", result[0].Name);
                Assert.Equal("A", result[1].Name);
                Assert.Equal("A", result[2].Name);
                Assert.Equal("A", result[3].Name);
                Assert.Equal("A", result[4].Name);
                Assert.Equal("A", result[5].Name);
                Assert.Equal("B", result[6].Name);
                Assert.Equal("B", result[7].Name);
                Assert.Equal("B", result[8].Name);
                Assert.Equal("B", result[9].Name);
                Assert.Equal("B", result[10].Name);
                Assert.Equal("B", result[11].Name);

                Assert.Equal("3", result[0].Description);
                Assert.Equal("3", result[1].Description);
                Assert.Equal("2", result[2].Description);
                Assert.Equal("2", result[3].Description);
                Assert.Equal("1", result[4].Description);
                Assert.Equal("1", result[5].Description);
                Assert.Equal("3", result[6].Description);
                Assert.Equal("3", result[7].Description);
                Assert.Equal("2", result[8].Description);
                Assert.Equal("2", result[9].Description);
                Assert.Equal("1", result[10].Description);
                Assert.Equal("1", result[11].Description);
            }

            [Fact]
            public void When_GettingBucketsOrderedByNameDescendingThenByDescriptionAscending_Expect_OrderedByNameDescendingThenByDescriptionAscending()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        context.Set<Bucket>().Add(bucket);
                    }

                    context.SaveChanges();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Name", false));
                ordering.Add(new OrderBy("Description", true));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, null, null, ordering);
                }

                // Assert
                Assert.Equal("B", result[0].Name);
                Assert.Equal("B", result[1].Name);
                Assert.Equal("B", result[2].Name);
                Assert.Equal("B", result[3].Name);
                Assert.Equal("B", result[4].Name);
                Assert.Equal("B", result[5].Name);
                Assert.Equal("A", result[6].Name);
                Assert.Equal("A", result[7].Name);
                Assert.Equal("A", result[8].Name);
                Assert.Equal("A", result[9].Name);
                Assert.Equal("A", result[10].Name);
                Assert.Equal("A", result[11].Name);

                Assert.Equal("1", result[0].Description);
                Assert.Equal("1", result[1].Description);
                Assert.Equal("2", result[2].Description);
                Assert.Equal("2", result[3].Description);
                Assert.Equal("3", result[4].Description);
                Assert.Equal("3", result[5].Description);
                Assert.Equal("1", result[6].Description);
                Assert.Equal("1", result[7].Description);
                Assert.Equal("2", result[8].Description);
                Assert.Equal("2", result[9].Description);
                Assert.Equal("3", result[10].Description);
                Assert.Equal("3", result[11].Description);
            }

            [Fact]
            public void When_GettingBucketsWithEmptyOrderedBy_Expect_DefaultOrdering()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        context.Set<Bucket>().Add(bucket);
                    }

                    context.SaveChanges();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.Get(bucket => true, null, null, ordering);
                }

                // Assert
                Assert.Equal("B", result[0].Name);
                Assert.Equal("A", result[1].Name);
                Assert.Equal("B", result[2].Name);
                Assert.Equal("B", result[3].Name);
                Assert.Equal("B", result[4].Name);
                Assert.Equal("A", result[5].Name);
                Assert.Equal("B", result[6].Name);
                Assert.Equal("A", result[7].Name);
                Assert.Equal("A", result[8].Name);
                Assert.Equal("A", result[9].Name);
                Assert.Equal("B", result[10].Name);
                Assert.Equal("A", result[11].Name);

                Assert.Equal("1", result[0].Description);
                Assert.Equal("3", result[1].Description);
                Assert.Equal("1", result[2].Description);
                Assert.Equal("3", result[3].Description);
                Assert.Equal("3", result[4].Description);
                Assert.Equal("3", result[5].Description);
                Assert.Equal("2", result[6].Description);
                Assert.Equal("2", result[7].Description);
                Assert.Equal("1", result[8].Description);
                Assert.Equal("2", result[9].Description);
                Assert.Equal("2", result[10].Description);
                Assert.Equal("1", result[11].Description);

                Assert.Equal(3, result[0].Size);
                Assert.Equal(9, result[1].Size);
                Assert.Equal(7, result[2].Size);
                Assert.Equal(10, result[3].Size);
                Assert.Equal(4, result[4].Size);
                Assert.Equal(6, result[5].Size);
                Assert.Equal(1, result[6].Size);
                Assert.Equal(11, result[7].Size);
                Assert.Equal(5, result[8].Size);
                Assert.Equal(12, result[9].Size);
                Assert.Equal(2, result[10].Size);
                Assert.Equal(8, result[11].Size);
            }
        }

        public class GetWithOrderingAsync : BucketRepositoryUnitTests
        {
            [Fact]
            public async Task When_GettingBucketsOrderedBySizeAscending_Expect_OrderedBySizeAscending()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        await context.Set<Bucket>().AddAsync(bucket);
                    }

                    await context.SaveChangesAsync();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Size", true));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null, ordering);
                }

                // Assert
                for (int i = 1; i <= 12; i++)
                {
                    Assert.Equal(i, result[i - 1].Size);
                }
            }

            [Fact]
            public async Task When_GettingBucketsOrderedBySizeDescending_Expect_OrderedBySizeDescending()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        await context.Set<Bucket>().AddAsync(bucket);
                    }

                    await context.SaveChangesAsync();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Size", false));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null, ordering);
                }

                // Assert
                for (int i = 1; i <= 12; i++)
                {
                    Assert.Equal(12 - (i - 1), result[i - 1].Size);
                }
            }

            [Fact]
            public async Task When_GettingBucketsOrderedBySizeWithLimit_Expect_OrderedLimitedBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        await context.Set<Bucket>().AddAsync(bucket);
                    }

                    await context.SaveChangesAsync();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Size", true));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, 6, null, ordering);
                }

                // Assert
                for (int i = 1; i <= 6; i++)
                {
                    Assert.Equal(i, result[i - 1].Size);
                }
            }

            [Fact]
            public async Task When_GettingBucketsOrderedBySizeWithOffset_Expect_OrderedOffsetBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        await context.Set<Bucket>().AddAsync(bucket);
                    }

                    await context.SaveChangesAsync();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Size", true));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, null, 6, ordering);
                }

                // Assert
                for (int i = 1; i <= 6; i++)
                {
                    Assert.Equal(i + 6, result[i - 1].Size);
                }
            }

            [Fact]
            public async Task When_GettingBucketsOrderedBySizeWithLimtAndOffset_Expect_OrderedLimitedAndOffsetBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        await context.Set<Bucket>().AddAsync(bucket);
                    }

                    await context.SaveChangesAsync();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Size", true));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, 6, 3, ordering);
                }

                // Assert
                for (int i = 1; i <= 6; i++)
                {
                    Assert.Equal(i + 3, result[i - 1].Size);
                }
            }

            [Fact]
            public async Task When_GettingBucketsOrderedByNameAscendingThenByDescriptionAscending_Expect_OrderedByNameAscendingThenByDescriptionAscending()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        await context.Set<Bucket>().AddAsync(bucket);
                    }

                    await context.SaveChangesAsync();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Name", true));
                ordering.Add(new OrderBy("Description", true));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null, ordering);
                }

                // Assert
                Assert.Equal("A", result[0].Name);
                Assert.Equal("A", result[1].Name);
                Assert.Equal("A", result[2].Name);
                Assert.Equal("A", result[3].Name);
                Assert.Equal("A", result[4].Name);
                Assert.Equal("A", result[5].Name);
                Assert.Equal("B", result[6].Name);
                Assert.Equal("B", result[7].Name);
                Assert.Equal("B", result[8].Name);
                Assert.Equal("B", result[9].Name);
                Assert.Equal("B", result[10].Name);
                Assert.Equal("B", result[11].Name);

                Assert.Equal("1", result[0].Description);
                Assert.Equal("1", result[1].Description);
                Assert.Equal("2", result[2].Description);
                Assert.Equal("2", result[3].Description);
                Assert.Equal("3", result[4].Description);
                Assert.Equal("3", result[5].Description);
                Assert.Equal("1", result[6].Description);
                Assert.Equal("1", result[7].Description);
                Assert.Equal("2", result[8].Description);
                Assert.Equal("2", result[9].Description);
                Assert.Equal("3", result[10].Description);
                Assert.Equal("3", result[11].Description);
            }

            [Fact]
            public async Task When_GettingBucketsOrderedByNameDescendingThenByDescriptionDescending_Expect_OrderedByNameDescendingThenByDescriptionDescending()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        await context.Set<Bucket>().AddAsync(bucket);
                    }

                    await context.SaveChangesAsync();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Name", false));
                ordering.Add(new OrderBy("Description", false));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null, ordering);
                }

                // Assert
                Assert.Equal("B", result[0].Name);
                Assert.Equal("B", result[1].Name);
                Assert.Equal("B", result[2].Name);
                Assert.Equal("B", result[3].Name);
                Assert.Equal("B", result[4].Name);
                Assert.Equal("B", result[5].Name);
                Assert.Equal("A", result[6].Name);
                Assert.Equal("A", result[7].Name);
                Assert.Equal("A", result[8].Name);
                Assert.Equal("A", result[9].Name);
                Assert.Equal("A", result[10].Name);
                Assert.Equal("A", result[11].Name);

                Assert.Equal("3", result[0].Description);
                Assert.Equal("3", result[1].Description);
                Assert.Equal("2", result[2].Description);
                Assert.Equal("2", result[3].Description);
                Assert.Equal("1", result[4].Description);
                Assert.Equal("1", result[5].Description);
                Assert.Equal("3", result[6].Description);
                Assert.Equal("3", result[7].Description);
                Assert.Equal("2", result[8].Description);
                Assert.Equal("2", result[9].Description);
                Assert.Equal("1", result[10].Description);
                Assert.Equal("1", result[11].Description);
            }

            [Fact]
            public async Task When_GettingBucketsOrderedByNameAscendingThenByDescriptionDescending_Expect_OrderedByNameAscendingThenByDescriptionDescending()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        await context.Set<Bucket>().AddAsync(bucket);
                    }

                    await context.SaveChangesAsync();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Name", true));
                ordering.Add(new OrderBy("Description", false));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null, ordering);
                }

                // Assert
                Assert.Equal("A", result[0].Name);
                Assert.Equal("A", result[1].Name);
                Assert.Equal("A", result[2].Name);
                Assert.Equal("A", result[3].Name);
                Assert.Equal("A", result[4].Name);
                Assert.Equal("A", result[5].Name);
                Assert.Equal("B", result[6].Name);
                Assert.Equal("B", result[7].Name);
                Assert.Equal("B", result[8].Name);
                Assert.Equal("B", result[9].Name);
                Assert.Equal("B", result[10].Name);
                Assert.Equal("B", result[11].Name);

                Assert.Equal("3", result[0].Description);
                Assert.Equal("3", result[1].Description);
                Assert.Equal("2", result[2].Description);
                Assert.Equal("2", result[3].Description);
                Assert.Equal("1", result[4].Description);
                Assert.Equal("1", result[5].Description);
                Assert.Equal("3", result[6].Description);
                Assert.Equal("3", result[7].Description);
                Assert.Equal("2", result[8].Description);
                Assert.Equal("2", result[9].Description);
                Assert.Equal("1", result[10].Description);
                Assert.Equal("1", result[11].Description);
            }

            [Fact]
            public async Task When_GettingBucketsOrderedByNameDescendingThenByDescriptionAscending_Expect_OrderedByNameDescendingThenByDescriptionAscending()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        await context.Set<Bucket>().AddAsync(bucket);
                    }

                    await context.SaveChangesAsync();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();
                ordering.Add(new OrderBy("Name", false));
                ordering.Add(new OrderBy("Description", true));

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null, ordering);
                }

                // Assert
                Assert.Equal("B", result[0].Name);
                Assert.Equal("B", result[1].Name);
                Assert.Equal("B", result[2].Name);
                Assert.Equal("B", result[3].Name);
                Assert.Equal("B", result[4].Name);
                Assert.Equal("B", result[5].Name);
                Assert.Equal("A", result[6].Name);
                Assert.Equal("A", result[7].Name);
                Assert.Equal("A", result[8].Name);
                Assert.Equal("A", result[9].Name);
                Assert.Equal("A", result[10].Name);
                Assert.Equal("A", result[11].Name);

                Assert.Equal("1", result[0].Description);
                Assert.Equal("1", result[1].Description);
                Assert.Equal("2", result[2].Description);
                Assert.Equal("2", result[3].Description);
                Assert.Equal("3", result[4].Description);
                Assert.Equal("3", result[5].Description);
                Assert.Equal("1", result[6].Description);
                Assert.Equal("1", result[7].Description);
                Assert.Equal("2", result[8].Description);
                Assert.Equal("2", result[9].Description);
                Assert.Equal("3", result[10].Description);
                Assert.Equal("3", result[11].Description);
            }

            [Fact]
            public async Task When_GettingBucketsWithEmptyOrderedBy_Expect_DefaultOrdering()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    foreach (Bucket bucket in this.testBuckets)
                    {
                        await context.Set<Bucket>().AddAsync(bucket);
                    }

                    await context.SaveChangesAsync();
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>();

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAsync(bucket => true, null, null, ordering);
                }

                // Assert
                Assert.Equal("B", result[0].Name);
                Assert.Equal("A", result[1].Name);
                Assert.Equal("B", result[2].Name);
                Assert.Equal("B", result[3].Name);
                Assert.Equal("B", result[4].Name);
                Assert.Equal("A", result[5].Name);
                Assert.Equal("B", result[6].Name);
                Assert.Equal("A", result[7].Name);
                Assert.Equal("A", result[8].Name);
                Assert.Equal("A", result[9].Name);
                Assert.Equal("B", result[10].Name);
                Assert.Equal("A", result[11].Name);

                Assert.Equal("1", result[0].Description);
                Assert.Equal("3", result[1].Description);
                Assert.Equal("1", result[2].Description);
                Assert.Equal("3", result[3].Description);
                Assert.Equal("3", result[4].Description);
                Assert.Equal("3", result[5].Description);
                Assert.Equal("2", result[6].Description);
                Assert.Equal("2", result[7].Description);
                Assert.Equal("1", result[8].Description);
                Assert.Equal("2", result[9].Description);
                Assert.Equal("2", result[10].Description);
                Assert.Equal("1", result[11].Description);

                Assert.Equal(3, result[0].Size);
                Assert.Equal(9, result[1].Size);
                Assert.Equal(7, result[2].Size);
                Assert.Equal(10, result[3].Size);
                Assert.Equal(4, result[4].Size);
                Assert.Equal(6, result[5].Size);
                Assert.Equal(1, result[6].Size);
                Assert.Equal(11, result[7].Size);
                Assert.Equal(5, result[8].Size);
                Assert.Equal(12, result[9].Size);
                Assert.Equal(2, result[10].Size);
                Assert.Equal(8, result[11].Size);
            }
        }

        public class Remove : BucketRepositoryUnitTests
        {
            [Fact]
            public void When_RemovingExistingBucket_Expect_Removed()
            {
                // Arrange
                Bucket bucket = new Bucket();

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(bucket);
                    context.SaveChanges();
                }

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    repository.Remove(bucket);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Null(context.Set<Bucket>().Find(bucket.Id));
                    Assert.Equal(0, context.Set<Bucket>().Count());
                }
            }

            [Fact]
            public void When_RemovingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                IBucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = Record.Exception(() => repository.Remove(null));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }
        }

        public class RemoveAsync : BucketRepositoryUnitTests
        {
            [Fact]
            public async Task When_RemovingExistingBucket_Expect_Removed()
            {
                // Arrange
                Bucket bucket = new Bucket();

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(bucket);
                    await context.SaveChangesAsync();
                }

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    await repository.RemoveAsync(bucket);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Null(context.Set<Bucket>().Find(bucket.Id));
                    Assert.Equal(0, context.Set<Bucket>().Count());
                }
            }

            [Fact]
            public async Task When_RemovingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                IBucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.RemoveAsync(null));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }
        }

        public class Update : BucketRepositoryUnitTests
        {
            [Fact]
            public void When_UpdatingExistingBucket_Expect_Updated()
            {
                // Arrange
                Bucket bucket = new Bucket();

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(bucket);
                    context.SaveChanges();
                }

                string newValue = Guid.NewGuid().ToString();

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    bucket.Name = newValue;
                    repository.Update(bucket);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Bucket result = context.Set<Bucket>().Find(bucket.Id);

                    Assert.NotEqual(bucket, result);
                    Assert.Equal(bucket.Id, result.Id);
                    Assert.Equal(newValue, result.Name);

                    Assert.Equal(1, context.Set<Bucket>().Count());
                }
            }

            [Fact]
            public void When_UpdatingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                IBucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = Record.Exception(() => repository.Update(null));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }
        }

        public class UpdateAsync : BucketRepositoryUnitTests
        {
            [Fact]
            public async Task When_UpdatingExistingBucket_Expect_Updated()
            {
                // Arrange
                Bucket bucket = new Bucket();

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(bucket);
                    await context.SaveChangesAsync();
                }

                string newValue = Guid.NewGuid().ToString();

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    bucket.Name = newValue;
                    await repository.UpdateAsync(bucket);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Bucket result = context.Set<Bucket>().Find(bucket.Id);

                    Assert.NotEqual(bucket, result);
                    Assert.Equal(bucket.Id, result.Id);
                    Assert.Equal(newValue, result.Name);

                    Assert.Equal(1, context.Set<Bucket>().Count());
                }
            }

            [Fact]
            public async Task When_UpdatingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                IBucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.UpdateAsync(null));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }
        }
    }
}
