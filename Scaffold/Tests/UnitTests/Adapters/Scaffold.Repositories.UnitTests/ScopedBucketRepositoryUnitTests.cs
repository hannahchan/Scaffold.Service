namespace Scaffold.Repositories.UnitTests;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scaffold.Domain.Aggregates.Bucket;
using Xunit;

public class ScopedBucketRepositoryUnitTests
{
    private readonly DbContextOptions<BucketContext> dbContextOptions;

    public ScopedBucketRepositoryUnitTests()
    {
        this.dbContextOptions = new DbContextOptionsBuilder<BucketContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    public class AddBucket : ScopedBucketRepositoryUnitTests
    {
        [Fact]
        public void When_AddingBucket_Expect_Saved()
        {
            // Arrange
            Bucket bucket = new Bucket();
            bucket.AddItem(new Item());

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                repository.Add(bucket);
            }

            // Assert
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Single(context.Buckets);
                Assert.Single(context.Items);

                Bucket result = context.Buckets.Find(bucket.Id);
                Assert.NotEqual(bucket, result);
                Assert.Equal(bucket.Id, result.Id);
                Assert.Single(result.Items);
            }
        }
    }

    public class AddBuckets : ScopedBucketRepositoryUnitTests
    {
        [Fact]
        public void When_AddingEnumerableOfBuckets_Expect_Saved()
        {
            // Arrange
            Bucket bucket1 = new Bucket { Name = "Bucket 1" };
            Bucket bucket2 = new Bucket { Name = "Bucket 2" };
            Bucket bucket3 = new Bucket { Name = "Bucket 3" };

            bucket1.AddItem(new Item { Name = "Item 1" });
            bucket2.AddItem(new Item { Name = "Item 2" });
            bucket3.AddItem(new Item { Name = "Item 3" });

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                repository.Add(new Bucket[] { bucket1, bucket2, bucket3 });
            }

            // Assert
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 2", bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name));

                Assert.Collection(
                    context.Items,
                    item => Assert.Equal("Item 1", item.Name),
                    item => Assert.Equal("Item 2", item.Name),
                    item => Assert.Equal("Item 3", item.Name));
            }
        }

        [Fact]
        public void When_AddingEmptyEnumerableOfBuckets_Expect_NoChange()
        {
            // Arrange
            Exception exception;

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                exception = Record.Exception(() => repository.Add(Array.Empty<Bucket>()));
            }

            // Assert
            Assert.Null(exception);

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Empty(context.Buckets);
            }
        }
    }

    public class AddBucketAsync : ScopedBucketRepositoryUnitTests
    {
        [Fact]
        public async Task When_AddingBucket_Expect_Saved()
        {
            // Arrange
            Bucket bucket = new Bucket();
            bucket.AddItem(new Item());

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                await repository.AddAsync(bucket);
            }

            // Assert
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Single(context.Buckets);
                Assert.Single(context.Items);

                Bucket result = context.Buckets.Find(bucket.Id);
                Assert.NotEqual(bucket, result);
                Assert.Equal(bucket.Id, result.Id);
                Assert.Single(result.Items);
            }
        }
    }

    public class AddBucketsAsync : ScopedBucketRepositoryUnitTests
    {
        [Fact]
        public async Task When_AddingEnumerableOfBuckets_Expect_Saved()
        {
            // Arrange
            Bucket bucket1 = new Bucket { Name = "Bucket 1" };
            Bucket bucket2 = new Bucket { Name = "Bucket 2" };
            Bucket bucket3 = new Bucket { Name = "Bucket 3" };

            bucket1.AddItem(new Item { Name = "Item 1" });
            bucket2.AddItem(new Item { Name = "Item 2" });
            bucket3.AddItem(new Item { Name = "Item 3" });

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                await repository.AddAsync(new Bucket[] { bucket1, bucket2, bucket3 });
            }

            // Assert
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 2", bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name));

                Assert.Collection(
                    context.Items,
                    item => Assert.Equal("Item 1", item.Name),
                    item => Assert.Equal("Item 2", item.Name),
                    item => Assert.Equal("Item 3", item.Name));
            }
        }

        [Fact]
        public async Task When_AddingEmptyEnumerableOfBuckets_Expect_NoChange()
        {
            // Arrange
            Exception exception;

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                exception = await Record.ExceptionAsync(() => repository.AddAsync(Array.Empty<Bucket>()));
            }

            // Assert
            Assert.Null(exception);

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Empty(context.Buckets);
            }
        }
    }

    public class RemoveBucket : ScopedBucketRepositoryUnitTests
    {
        [Fact]
        public void When_RemovingExistingBucket_Expect_Removed()
        {
            // Arrange
            Bucket bucket1 = new Bucket { Name = "Bucket 1" };
            Bucket bucket2 = new Bucket { Name = "Bucket 2" };
            Bucket bucket3 = new Bucket { Name = "Bucket 3" };
            Bucket bucket4 = new Bucket { Name = "Bucket 4" };
            Bucket bucket5 = new Bucket { Name = "Bucket 5" };

            bucket1.AddItem(new Item { Name = "Item 1" });
            bucket2.AddItem(new Item { Name = "Item 2" });
            bucket3.AddItem(new Item { Name = "Item 3" });
            bucket4.AddItem(new Item { Name = "Item 4" });
            bucket5.AddItem(new Item { Name = "Item 5" });

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                context.Buckets.AddRange(new Bucket[] { bucket1, bucket2, bucket3, bucket4, bucket5 });
                context.SaveChanges();
            }

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                repository.Remove(bucket3);
            }

            // Assert
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Null(context.Buckets.Find(bucket3.Id));

                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 2", bucket.Name),
                    bucket => Assert.Equal("Bucket 4", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));

                Assert.Collection(
                    context.Items,
                    item => Assert.Equal("Item 1", item.Name),
                    item => Assert.Equal("Item 2", item.Name),
                    item => Assert.Equal("Item 4", item.Name),
                    item => Assert.Equal("Item 5", item.Name));
            }
        }
    }

    public class RemoveBuckets : ScopedBucketRepositoryUnitTests
    {
        [Fact]
        public void When_RemovingEnumerableOfBuckets_Expect_Removed()
        {
            // Arrange
            Bucket bucket1 = new Bucket { Name = "Bucket 1" };
            Bucket bucket2 = new Bucket { Name = "Bucket 2" };
            Bucket bucket3 = new Bucket { Name = "Bucket 3" };
            Bucket bucket4 = new Bucket { Name = "Bucket 4" };
            Bucket bucket5 = new Bucket { Name = "Bucket 5" };

            bucket1.AddItem(new Item { Name = "Item 1" });
            bucket2.AddItem(new Item { Name = "Item 2" });
            bucket3.AddItem(new Item { Name = "Item 3" });
            bucket4.AddItem(new Item { Name = "Item 4" });
            bucket5.AddItem(new Item { Name = "Item 5" });

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                context.Buckets.AddRange(new Bucket[] { bucket1, bucket2, bucket3, bucket4, bucket5 });
                context.SaveChanges();
            }

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                repository.Remove(new Bucket[] { bucket2, bucket4 });
            }

            // Assert
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Null(context.Buckets.Find(bucket2.Id));
                Assert.Null(context.Buckets.Find(bucket4.Id));

                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));

                Assert.Collection(
                    context.Items,
                    bucket => Assert.Equal("Item 1", bucket.Name),
                    bucket => Assert.Equal("Item 3", bucket.Name),
                    bucket => Assert.Equal("Item 5", bucket.Name));
            }
        }

        [Fact]
        public void When_RemovingEmptyEnumerableOfBuckets_Expect_NoChange()
        {
            // Arrange
            Bucket bucket1 = new Bucket { Name = "Bucket 1" };
            Bucket bucket2 = new Bucket { Name = "Bucket 2" };
            Bucket bucket3 = new Bucket { Name = "Bucket 3" };
            Bucket bucket4 = new Bucket { Name = "Bucket 4" };
            Bucket bucket5 = new Bucket { Name = "Bucket 5" };

            bucket1.AddItem(new Item { Name = "Item 1" });
            bucket2.AddItem(new Item { Name = "Item 2" });
            bucket3.AddItem(new Item { Name = "Item 3" });
            bucket4.AddItem(new Item { Name = "Item 4" });
            bucket5.AddItem(new Item { Name = "Item 5" });

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                context.Buckets.AddRange(new Bucket[] { bucket1, bucket2, bucket3, bucket4, bucket5 });
                context.SaveChanges();
            }

            Exception exception;

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                exception = Record.Exception(() => repository.Remove(Array.Empty<Bucket>()));
            }

            // Assert
            Assert.Null(exception);

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 2", bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name),
                    bucket => Assert.Equal("Bucket 4", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));

                Assert.Collection(
                    context.Items,
                    bucket => Assert.Equal("Item 1", bucket.Name),
                    bucket => Assert.Equal("Item 2", bucket.Name),
                    bucket => Assert.Equal("Item 3", bucket.Name),
                    bucket => Assert.Equal("Item 4", bucket.Name),
                    bucket => Assert.Equal("Item 5", bucket.Name));
            }
        }
    }

    public class RemoveBucketAsync : ScopedBucketRepositoryUnitTests
    {
        [Fact]
        public async Task When_RemovingExistingBucket_Expect_Removed()
        {
            // Arrange
            Bucket bucket1 = new Bucket { Name = "Bucket 1" };
            Bucket bucket2 = new Bucket { Name = "Bucket 2" };
            Bucket bucket3 = new Bucket { Name = "Bucket 3" };
            Bucket bucket4 = new Bucket { Name = "Bucket 4" };
            Bucket bucket5 = new Bucket { Name = "Bucket 5" };

            bucket1.AddItem(new Item { Name = "Item 1" });
            bucket2.AddItem(new Item { Name = "Item 2" });
            bucket3.AddItem(new Item { Name = "Item 3" });
            bucket4.AddItem(new Item { Name = "Item 4" });
            bucket5.AddItem(new Item { Name = "Item 5" });

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                context.Buckets.AddRange(new Bucket[] { bucket1, bucket2, bucket3, bucket4, bucket5 });
                context.SaveChanges();
            }

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                await repository.RemoveAsync(bucket3);
            }

            // Assert
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Null(context.Buckets.Find(bucket3.Id));

                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 2", bucket.Name),
                    bucket => Assert.Equal("Bucket 4", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));

                Assert.Collection(
                    context.Items,
                    item => Assert.Equal("Item 1", item.Name),
                    item => Assert.Equal("Item 2", item.Name),
                    item => Assert.Equal("Item 4", item.Name),
                    item => Assert.Equal("Item 5", item.Name));
            }
        }
    }

    public class RemoveBucketsAsync : ScopedBucketRepositoryUnitTests
    {
        [Fact]
        public async Task When_RemovingEnumerableOfBuckets_Expect_Removed()
        {
            // Arrange
            Bucket bucket1 = new Bucket { Name = "Bucket 1" };
            Bucket bucket2 = new Bucket { Name = "Bucket 2" };
            Bucket bucket3 = new Bucket { Name = "Bucket 3" };
            Bucket bucket4 = new Bucket { Name = "Bucket 4" };
            Bucket bucket5 = new Bucket { Name = "Bucket 5" };

            bucket1.AddItem(new Item { Name = "Item 1" });
            bucket2.AddItem(new Item { Name = "Item 2" });
            bucket3.AddItem(new Item { Name = "Item 3" });
            bucket4.AddItem(new Item { Name = "Item 4" });
            bucket5.AddItem(new Item { Name = "Item 5" });

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                context.Buckets.AddRange(new Bucket[] { bucket1, bucket2, bucket3, bucket4, bucket5 });
                context.SaveChanges();
            }

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                await repository.RemoveAsync(new Bucket[] { bucket2, bucket4 });
            }

            // Assert
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Null(context.Buckets.Find(bucket2.Id));
                Assert.Null(context.Buckets.Find(bucket4.Id));

                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));

                Assert.Collection(
                    context.Items,
                    bucket => Assert.Equal("Item 1", bucket.Name),
                    bucket => Assert.Equal("Item 3", bucket.Name),
                    bucket => Assert.Equal("Item 5", bucket.Name));
            }
        }

        [Fact]
        public async Task When_RemovingEmptyEnumerableOfBuckets_Expect_NoChange()
        {
            // Arrange
            Bucket bucket1 = new Bucket { Name = "Bucket 1" };
            Bucket bucket2 = new Bucket { Name = "Bucket 2" };
            Bucket bucket3 = new Bucket { Name = "Bucket 3" };
            Bucket bucket4 = new Bucket { Name = "Bucket 4" };
            Bucket bucket5 = new Bucket { Name = "Bucket 5" };

            bucket1.AddItem(new Item { Name = "Item 1" });
            bucket2.AddItem(new Item { Name = "Item 2" });
            bucket3.AddItem(new Item { Name = "Item 3" });
            bucket4.AddItem(new Item { Name = "Item 4" });
            bucket5.AddItem(new Item { Name = "Item 5" });

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                context.Buckets.AddRange(new Bucket[] { bucket1, bucket2, bucket3, bucket4, bucket5 });
                context.SaveChanges();
            }

            Exception exception;

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                exception = await Record.ExceptionAsync(() => repository.RemoveAsync(Array.Empty<Bucket>()));
            }

            // Assert
            Assert.Null(exception);

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 2", bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name),
                    bucket => Assert.Equal("Bucket 4", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));

                Assert.Collection(
                    context.Items,
                    bucket => Assert.Equal("Item 1", bucket.Name),
                    bucket => Assert.Equal("Item 2", bucket.Name),
                    bucket => Assert.Equal("Item 3", bucket.Name),
                    bucket => Assert.Equal("Item 4", bucket.Name),
                    bucket => Assert.Equal("Item 5", bucket.Name));
            }
        }
    }

    public class UpdateBucket : ScopedBucketRepositoryUnitTests
    {
        [Fact]
        public void When_UpdatingExistingBucket_Expect_Updated()
        {
            // Arrange
            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
                new Bucket { Name = "Bucket 4" },
                new Bucket { Name = "Bucket 5" },
            };

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                context.Buckets.AddRange(buckets);
                context.SaveChanges();
            }

            string newValue = Guid.NewGuid().ToString();

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                buckets[1].Name = newValue;
                repository.Update(buckets[1]);
            }

            // Assert
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal(newValue, bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name),
                    bucket => Assert.Equal("Bucket 4", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));
            }
        }
    }

    public class UpdateBuckets : ScopedBucketRepositoryUnitTests
    {
        [Fact]
        public void When_UpdatingMultipleExistingBuckets_Expect_Updated()
        {
            // Arrange
            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
                new Bucket { Name = "Bucket 4" },
                new Bucket { Name = "Bucket 5" },
            };

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                context.Buckets.AddRange(buckets);
                context.SaveChanges();
            }

            string newValue1 = Guid.NewGuid().ToString();
            string newValue2 = Guid.NewGuid().ToString();

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                buckets[1].Name = newValue1;
                buckets[3].Name = newValue2;
                repository.Update(new Bucket[] { buckets[1], buckets[3] });
            }

            // Assert
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal(newValue1, bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name),
                    bucket => Assert.Equal(newValue2, bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));
            }
        }

        [Fact]
        public void When_UpdatingMultipleBucketsWithEmptyEnumerableOfBuckets_Expect_NoChange()
        {
            // Arrange
            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
                new Bucket { Name = "Bucket 4" },
                new Bucket { Name = "Bucket 5" },
            };

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                context.Buckets.AddRange(buckets);
                context.SaveChanges();
            }

            Exception exception;

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                exception = Record.Exception(() => repository.Update(Array.Empty<Bucket>()));
            }

            // Assert
            Assert.Null(exception);

            using (BucketContext context = new BucketContext(this.dbContextOptions))
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

    public class UpdateBucketAsync : ScopedBucketRepositoryUnitTests
    {
        [Fact]
        public async Task When_UpdatingExistingBucket_Expect_Updated()
        {
            // Arrange
            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
                new Bucket { Name = "Bucket 4" },
                new Bucket { Name = "Bucket 5" },
            };

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                context.Buckets.AddRange(buckets);
                context.SaveChanges();
            }

            string newValue = Guid.NewGuid().ToString();

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                buckets[1].Name = newValue;
                await repository.UpdateAsync(buckets[1]);
            }

            // Assert
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal(newValue, bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name),
                    bucket => Assert.Equal("Bucket 4", bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));
            }
        }
    }

    public class UpdateBucketsAsync : ScopedBucketRepositoryUnitTests
    {
        [Fact]
        public async Task When_UpdatingMultipleExistingBuckets_Expect_Updated()
        {
            // Arrange
            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
                new Bucket { Name = "Bucket 4" },
                new Bucket { Name = "Bucket 5" },
            };

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                context.Buckets.AddRange(buckets);
                context.SaveChanges();
            }

            string newValue1 = Guid.NewGuid().ToString();
            string newValue2 = Guid.NewGuid().ToString();

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                buckets[1].Name = newValue1;
                buckets[3].Name = newValue2;
                await repository.UpdateAsync(new Bucket[] { buckets[1], buckets[3] });
            }

            // Assert
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                Assert.Collection(
                    context.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal(newValue1, bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name),
                    bucket => Assert.Equal(newValue2, bucket.Name),
                    bucket => Assert.Equal("Bucket 5", bucket.Name));
            }
        }

        [Fact]
        public async Task When_UpdatingMultipleBucketsWithEmptyEnumerableOfBuckets_Expect_NoChange()
        {
            // Arrange
            Bucket[] buckets =
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
                new Bucket { Name = "Bucket 4" },
                new Bucket { Name = "Bucket 5" },
            };

            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                context.Buckets.AddRange(buckets);
                context.SaveChanges();
            }

            Exception exception;

            // Act
            using (BucketContext context = new BucketContext(this.dbContextOptions))
            {
                ScopedBucketRepository repository = new ScopedBucketRepository(context);
                exception = await Record.ExceptionAsync(() => repository.UpdateAsync(Array.Empty<Bucket>()));
            }

            // Assert
            Assert.Null(exception);

            using (BucketContext context = new BucketContext(this.dbContextOptions))
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
}
