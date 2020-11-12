namespace Scaffold.Repositories.PostgreSQL.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Domain.Aggregates.Bucket;
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
                    BucketRepository repository = new BucketRepository(context);
                    repository.Add(bucket);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Bucket result = context.Buckets.Find(bucket.Id);

                    Assert.Equal(1, context.Buckets.Count());
                    Assert.NotEqual(bucket, result);
                    Assert.Equal(bucket.Id, result.Id);
                }
            }

            [Fact]
            public void When_AddingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = Record.Exception(() => repository.Add(null));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("bucket", argumentNullException.ParamName);
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
                    BucketRepository repository = new BucketRepository(context);
                    await repository.AddAsync(bucket);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Bucket result = context.Buckets.Find(bucket.Id);

                    Assert.Equal(1, context.Buckets.Count());
                    Assert.NotEqual(bucket, result);
                    Assert.Equal(bucket.Id, result.Id);
                }
            }

            [Fact]
            public async Task When_AddingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.AddAsync(null));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("bucket", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }

            [Fact(Skip = "Not testable with Entity Framework Core In-Memory Database")]
            public async Task When_AddingBucketAndCancellationIsRequested_Expect_OperationCanceledException()
            {
                // Arrange
                Bucket bucket = new Bucket();

                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.AddAsync(bucket, new CancellationToken(true)));

                // Assert
                Assert.IsType<OperationCanceledException>(exception);
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
                    context.Buckets.Add(bucket);
                    context.SaveChanges();
                }

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    repository.Remove(bucket);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Null(context.Buckets.Find(bucket.Id));
                    Assert.Equal(0, context.Buckets.Count());
                }
            }

            [Fact]
            public void When_RemovingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = Record.Exception(() => repository.Remove(null));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("bucket", argumentNullException.ParamName);
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
                    context.Buckets.Add(bucket);
                    await context.SaveChangesAsync();
                }

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    await repository.RemoveAsync(bucket);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Null(context.Buckets.Find(bucket.Id));
                    Assert.Equal(0, context.Buckets.Count());
                }
            }

            [Fact]
            public async Task When_RemovingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.RemoveAsync(null));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("bucket", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }

            [Fact(Skip = "Not testable with Entity Framework Core In-Memory Database")]
            public async Task When_RemovingBucketAndCancellationIsRequested_Expect_OperationCanceledException()
            {
                // Arrange
                Bucket bucket = new Bucket();

                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.RemoveAsync(bucket, new CancellationToken(true)));

                // Assert
                Assert.IsType<OperationCanceledException>(exception);
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
                    context.Buckets.Add(bucket);
                    context.SaveChanges();
                }

                string newValue = Guid.NewGuid().ToString();

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    bucket.Name = newValue;
                    repository.Update(bucket);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Bucket result = context.Buckets.Find(bucket.Id);

                    Assert.NotEqual(bucket, result);
                    Assert.Equal(bucket.Id, result.Id);
                    Assert.Equal(newValue, result.Name);

                    Assert.Equal(1, context.Buckets.Count());
                }
            }

            [Fact]
            public void When_UpdatingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = Record.Exception(() => repository.Update(null));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("bucket", argumentNullException.ParamName);
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
                    context.Buckets.Add(bucket);
                    await context.SaveChangesAsync();
                }

                string newValue = Guid.NewGuid().ToString();

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    bucket.Name = newValue;
                    await repository.UpdateAsync(bucket);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Bucket result = context.Buckets.Find(bucket.Id);

                    Assert.NotEqual(bucket, result);
                    Assert.Equal(bucket.Id, result.Id);
                    Assert.Equal(newValue, result.Name);

                    Assert.Equal(1, context.Buckets.Count());
                }
            }

            [Fact]
            public async Task When_UpdatingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.UpdateAsync(null));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("bucket", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }

            [Fact(Skip = "Not testable with Entity Framework Core In-Memory Database")]
            public async Task When_UpdatingBucketAndCancellationIsRequested_Expect_OperationCanceledException()
            {
                // Arrange
                Bucket bucket = new Bucket();

                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.UpdateAsync(bucket, new CancellationToken(true)));

                // Assert
                Assert.IsType<OperationCanceledException>(exception);
            }
        }
    }
}
