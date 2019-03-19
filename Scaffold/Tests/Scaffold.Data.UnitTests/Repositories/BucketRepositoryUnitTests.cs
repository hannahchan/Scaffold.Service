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
        }

        public class GetAll : BucketRepositoryUnitTests
        {
            [Fact]
            public void When_GettingExistingBuckets_Expect_ExistingBuckets()
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
                    result = repository.GetAll();
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(3, result.Count);
            }

            [Fact]
            public void When_GettingNonExistingBuckets_Expect_EmptyList()
            {
                // Arrange
                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = repository.GetAll();
                }

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }

        public class GetAllAsync : BucketRepositoryUnitTests
        {
            [Fact]
            public async Task When_GettingExistingBuckets_Expect_ExistingBuckets()
            {
                // Arrange
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    context.Set<Bucket>().Add(new Bucket());
                    context.Set<Bucket>().Add(new Bucket());
                    context.Set<Bucket>().Add(new Bucket());

                    await context.SaveChangesAsync();
                }

                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAllAsync();
                }

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(3, result.Count);
            }

            [Fact]
            public async Task When_GettingNonExistingBuckets_Expect_EmptyList()
            {
                // Arrange
                IList<Bucket> result;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    IBucketRepository repository = new BucketRepository(context);
                    result = await repository.GetAllAsync();
                }

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
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
