namespace Scaffold.Repositories.UnitTests
{
    using System;
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

        public class AddBucket : BucketRepositoryUnitTests
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
                    Assert.Single(context.Buckets);

                    Bucket result = context.Buckets.Find(bucket.Id);
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
                Exception exception = Record.Exception(() => repository.Add(null as Bucket));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("bucket", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }
        }

        public class AddBuckets : BucketRepositoryUnitTests
        {
            [Fact]
            public void When_AddingEnumerableOfBuckets_Expect_Saved()
            {
                // Arrange
                Bucket[] buckets =
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                };

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    repository.Add(buckets);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Collection(
                        context.Buckets,
                        bucket => Assert.Equal("Bucket 1", bucket.Name),
                        bucket => Assert.Equal("Bucket 2", bucket.Name),
                        bucket => Assert.Equal("Bucket 3", bucket.Name));
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
                    BucketRepository repository = new BucketRepository(context);
                    exception = Record.Exception(() => repository.Add(Array.Empty<Bucket>()));
                }

                // Assert
                Assert.Null(exception);

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Empty(context.Buckets);
                }
            }

            [Fact]
            public void When_AddingNullEnumerable_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = Record.Exception(() => repository.Add(null as Bucket[]));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("buckets", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }

            [Fact]
            public void When_AddingEnumerableOfBucketsWithNullBucket_Expect_ArgumentException()
            {
                // Arrange
                Bucket[] buckets =
                {
                    new Bucket { Name = "Bucket 1" },
                    null,
                    new Bucket { Name = "Bucket 3" },
                };

                Exception exception;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    exception = Record.Exception(() => repository.Add(buckets));
                }

                // Assert
                ArgumentException argumentException = Assert.IsType<ArgumentException>(exception);
                Assert.Equal("buckets", argumentException.ParamName);
                Assert.Equal("Enumerable cannot contain null. (Parameter 'buckets')", argumentException.Message);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Empty(context.Buckets);
                }
            }
        }

        public class AddBucketAsync : BucketRepositoryUnitTests
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
                    Assert.Single(context.Buckets);

                    Bucket result = context.Buckets.Find(bucket.Id);
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
                Exception exception = await Record.ExceptionAsync(() => repository.AddAsync(null as Bucket));

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

        public class AddBucketsAsync : BucketRepositoryUnitTests
        {
            [Fact]
            public async Task When_AddingEnumerableOfBuckets_Expect_Saved()
            {
                // Arrange
                Bucket[] buckets =
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                };

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    await repository.AddAsync(buckets);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Collection(
                        context.Buckets,
                        bucket => Assert.Equal("Bucket 1", bucket.Name),
                        bucket => Assert.Equal("Bucket 2", bucket.Name),
                        bucket => Assert.Equal("Bucket 3", bucket.Name));
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
                    BucketRepository repository = new BucketRepository(context);
                    exception = await Record.ExceptionAsync(() => repository.AddAsync(Array.Empty<Bucket>()));
                }

                // Assert
                Assert.Null(exception);

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Empty(context.Buckets);
                }
            }

            [Fact]
            public async Task When_AddingNullEnumerable_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.AddAsync(null as Bucket[]));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("buckets", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }

            [Fact]
            public async Task When_AddingEnumerableOfBucketsWithNullBucket_Expect_ArgumentException()
            {
                // Arrange
                Bucket[] buckets =
                {
                    new Bucket { Name = "Bucket 1" },
                    null,
                    new Bucket { Name = "Bucket 3" },
                };

                Exception exception;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    exception = await Record.ExceptionAsync(() => repository.AddAsync(buckets));
                }

                // Assert
                ArgumentException argumentException = Assert.IsType<ArgumentException>(exception);
                Assert.Equal("buckets", argumentException.ParamName);
                Assert.Equal("Enumerable cannot contain null. (Parameter 'buckets')", argumentException.Message);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Empty(context.Buckets);
                }
            }

            [Fact(Skip = "Not testable with Entity Framework Core In-Memory Database")]
            public async Task When_AddingEnumerableOfBucketsAndCancellationIsRequested_Expect_OperationCanceledException()
            {
                // Arrange
                Bucket[] buckets =
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                };

                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.AddAsync(buckets, new CancellationToken(true)));

                // Assert
                Assert.IsType<OperationCanceledException>(exception);
            }
        }

        public class RemoveBucket : BucketRepositoryUnitTests
        {
            [Fact]
            public void When_RemovingExistingBucket_Expect_Removed()
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

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    repository.Remove(buckets[2]);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
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

            [Fact]
            public void When_RemovingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = Record.Exception(() => repository.Remove(null as Bucket));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("bucket", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }
        }

        public class RemoveBuckets : BucketRepositoryUnitTests
        {
            [Fact]
            public void When_RemovingEnumerableOfBuckets_Expect_Removed()
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

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    repository.Remove(new Bucket[] { buckets[1], buckets[3] });
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
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
                    BucketRepository repository = new BucketRepository(context);
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
                }
            }

            [Fact]
            public void When_RemovingNullEnumerable_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = Record.Exception(() => repository.Remove(null as Bucket[]));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("buckets", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }

            [Fact]
            public void When_RemovingEnumerableOfBucketsWithNullBucket_Expect_ArgumentException()
            {
                // Arrange
                Bucket[] buckets =
                {
                    new Bucket { Name = "Bucket 1" },
                    null,
                    new Bucket { Name = "Bucket 3" },
                };

                Exception exception;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    exception = Record.Exception(() => repository.Remove(buckets));
                }

                // Assert
                ArgumentException argumentException = Assert.IsType<ArgumentException>(exception);
                Assert.Equal("buckets", argumentException.ParamName);
                Assert.Equal("Enumerable cannot contain null. (Parameter 'buckets')", argumentException.Message);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Empty(context.Buckets);
                }
            }
        }

        public class RemoveBucketAsync : BucketRepositoryUnitTests
        {
            [Fact]
            public async Task When_RemovingExistingBucket_Expect_Removed()
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

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    await repository.RemoveAsync(buckets[2]);
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
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

            [Fact]
            public async Task When_RemovingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.RemoveAsync(null as Bucket));

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

        public class RemoveBucketsAsync : BucketRepositoryUnitTests
        {
            [Fact]
            public async Task When_RemovingEnumerableOfBuckets_Expect_Removed()
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

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    await repository.RemoveAsync(new Bucket[] { buckets[1], buckets[3] });
                }

                // Assert
                using (BucketContext context = new BucketContext(this.dbContextOptions))
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
                    BucketRepository repository = new BucketRepository(context);
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
                }
            }

            [Fact]
            public async Task When_RemovingNullEnumerable_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.RemoveAsync(null as Bucket[]));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("buckets", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }

            [Fact]
            public async Task When_RemovingEnumerableOfBucketsWithNullBucket_Expect_ArgumentException()
            {
                // Arrange
                Bucket[] buckets =
                {
                    new Bucket { Name = "Bucket 1" },
                    null,
                    new Bucket { Name = "Bucket 3" },
                };

                Exception exception;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    exception = await Record.ExceptionAsync(() => repository.RemoveAsync(buckets));
                }

                // Assert
                ArgumentException argumentException = Assert.IsType<ArgumentException>(exception);
                Assert.Equal("buckets", argumentException.ParamName);
                Assert.Equal("Enumerable cannot contain null. (Parameter 'buckets')", argumentException.Message);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Empty(context.Buckets);
                }
            }

            [Fact(Skip = "Not testable with Entity Framework Core In-Memory Database")]
            public async Task When_RemovingEnumerableOfBucketsAndCancellationIsRequested_Expect_OperationCanceledException()
            {
                // Arrange
                Bucket[] buckets =
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                };

                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.RemoveAsync(buckets, new CancellationToken(true)));

                // Assert
                Assert.IsType<OperationCanceledException>(exception);
            }
        }

        public class UpdateBucket : BucketRepositoryUnitTests
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
                    BucketRepository repository = new BucketRepository(context);
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

            [Fact]
            public void When_UpdatingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = Record.Exception(() => repository.Update(null as Bucket));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("bucket", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }
        }

        public class UpdateBuckets : BucketRepositoryUnitTests
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
                    BucketRepository repository = new BucketRepository(context);
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
                    BucketRepository repository = new BucketRepository(context);
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

            [Fact]
            public void When_UpdatingMultipleBucketsWithNullEnumerable_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = Record.Exception(() => repository.Update(null as Bucket[]));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("buckets", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }

            [Fact]
            public void When_UpdatingMultipleBucketsWithEnumerableOfBucketsWithNullBucket_Expect_ArgumentException()
            {
                // Arrange
                Bucket[] buckets =
                {
                    new Bucket { Name = "Bucket 1" },
                    null,
                    new Bucket { Name = "Bucket 3" },
                };

                Exception exception;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    exception = Record.Exception(() => repository.Update(buckets));
                }

                // Assert
                ArgumentException argumentException = Assert.IsType<ArgumentException>(exception);
                Assert.Equal("buckets", argumentException.ParamName);
                Assert.Equal("Enumerable cannot contain null. (Parameter 'buckets')", argumentException.Message);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Empty(context.Buckets);
                }
            }
        }

        public class UpdateBucketAsync : BucketRepositoryUnitTests
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
                    BucketRepository repository = new BucketRepository(context);
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

            [Fact]
            public async Task When_UpdatingNull_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.UpdateAsync(null as Bucket));

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

        public class UpdateBucketsAsync : BucketRepositoryUnitTests
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
                    BucketRepository repository = new BucketRepository(context);
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
                    BucketRepository repository = new BucketRepository(context);
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

            [Fact]
            public async Task When_UpdatingMultipleBucketsWithNullEnumerable_Expect_ArgumentNullException()
            {
                // Arrange
                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.UpdateAsync(null as Bucket[]));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("buckets", argumentNullException.ParamName);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);
            }

            [Fact]
            public async Task When_UpdatingMultipleBucketsWithEnumerableOfBucketsWithNullBucket_Expect_ArgumentException()
            {
                // Arrange
                Bucket[] buckets =
                {
                    new Bucket { Name = "Bucket 1" },
                    null,
                    new Bucket { Name = "Bucket 3" },
                };

                Exception exception;

                // Act
                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    BucketRepository repository = new BucketRepository(context);
                    exception = await Record.ExceptionAsync(() => repository.UpdateAsync(buckets));
                }

                // Assert
                ArgumentException argumentException = Assert.IsType<ArgumentException>(exception);
                Assert.Equal("buckets", argumentException.ParamName);
                Assert.Equal("Enumerable cannot contain null. (Parameter 'buckets')", argumentException.Message);
                Assert.Equal(typeof(BucketRepository).Assembly.GetName().Name, exception.Source);

                using (BucketContext context = new BucketContext(this.dbContextOptions))
                {
                    Assert.Empty(context.Buckets);
                }
            }

            [Fact(Skip = "Not testable with Entity Framework Core In-Memory Database")]
            public async Task When_UpdatingMultipleBucketAndCancellationIsRequested_Expect_OperationCanceledException()
            {
                // Arrange
                Bucket[] buckets =
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                };

                BucketContext context = new BucketContext(this.dbContextOptions);
                BucketRepository repository = new BucketRepository(context);

                // Act
                Exception exception = await Record.ExceptionAsync(() => repository.UpdateAsync(buckets, new CancellationToken(true)));

                // Assert
                Assert.IsType<OperationCanceledException>(exception);
            }
        }
    }
}
