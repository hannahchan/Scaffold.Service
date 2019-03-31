namespace Scaffold.Data.UnitTests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Interfaces;
    using Scaffold.Data;
    using Scaffold.Data.Repositories;
    using Scaffold.Domain.Entities;
    using Xunit;

    public class BucketRepositoryUnitTests
    {
        private readonly DbContextOptions<BucketContext> dbContextOptions;

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
