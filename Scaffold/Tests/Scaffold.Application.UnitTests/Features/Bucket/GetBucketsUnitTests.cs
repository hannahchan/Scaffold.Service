namespace Scaffold.Application.UnitTests.Features.Bucket
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.Application.Interfaces;
    using Scaffold.Data;
    using Scaffold.Data.Repositories;
    using Scaffold.Domain.Entities;
    using Xunit;

    public class GetBucketsUnitTests
    {
        private readonly IBucketRepository repository;

        public GetBucketsUnitTests()
        {
            BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            this.repository = new BucketRepository(context);
        }

        public class Query
        {
            [Fact]
            public void When_InstantiatingNewQuery_Expect_PredicateNotNull()
            {
                // Arrange
                GetBuckets.Query query;

                // Act
                query = new GetBuckets.Query();

                // Assert
                Assert.NotNull(query.Predicate);
                Assert.Null(query.Limit);
                Assert.Null(query.Offset);
            }

            [Fact]
            public void When_InstantiatingNewQueryWithPredicate_Expect_Predicate()
            {
                // Arrange
                Expression<Func<Bucket, bool>> predicate = bucket => false;
                GetBuckets.Query query;

                // Act
                query = new GetBuckets.Query(predicate);

                // Assert
                Assert.Equal(predicate, query.Predicate);
                Assert.Null(query.Limit);
                Assert.Null(query.Offset);
            }

            [Fact]
            public void When_InstantiatingNewQueryWithNullPredicate_Expect_ArgumentNullException()
            {
                // Arrange
                Exception exception;

                // Act
                exception = Record.Exception(() => new GetBuckets.Query(null));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
            }
        }

        public class Handler : GetBucketsUnitTests
        {
            [Fact]
            public async Task When_GettingBuckets_Expect_AllBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket());
                await this.repository.AddAsync(new Bucket());
                await this.repository.AddAsync(new Bucket());

                GetBuckets.Query query = new GetBuckets.Query();
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default(CancellationToken));

                // Assert
                Assert.NotNull(response.Buckets);
                Assert.NotEmpty(response.Buckets);
                Assert.Equal(3, response.Buckets.Count);
            }

            [Fact]
            public async Task When_GettingNonExistingBuckets_Expect_EmptyList()
            {
                // Arrange
                GetBuckets.Query query = new GetBuckets.Query();
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default(CancellationToken));

                // Assert
                Assert.NotNull(response.Buckets);
                Assert.Empty(response.Buckets);
            }

            [Fact]
            public async Task When_GettingBucketsWithPredicate_Expect_SomeBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket { Size = 1 });
                await this.repository.AddAsync(new Bucket { Size = 2 });
                await this.repository.AddAsync(new Bucket { Size = 3 });
                await this.repository.AddAsync(new Bucket { Size = 5 });
                await this.repository.AddAsync(new Bucket { Size = 8 });

                GetBuckets.Query query = new GetBuckets.Query(bucket => bucket.Size == 2 || bucket.Size == 5);
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default(CancellationToken));

                // Assert
                Assert.NotNull(response.Buckets);
                Assert.NotEmpty(response.Buckets);
                Assert.Equal(2, response.Buckets.Count);
            }

            [Fact]
            public async Task When_GettingBucketsWithNoLimit_Expect_AllBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket { Name = "Bucket 1" });
                await this.repository.AddAsync(new Bucket { Name = "Bucket 2" });
                await this.repository.AddAsync(new Bucket { Name = "Bucket 3" });

                GetBuckets.Query query = new GetBuckets.Query { Limit = null };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default(CancellationToken));

                // Assert
                Assert.NotNull(response.Buckets);
                Assert.NotEmpty(response.Buckets);
                Assert.Equal(3, response.Buckets.Count);
                Assert.Equal("Bucket 1", response.Buckets[0].Name);
                Assert.Equal("Bucket 2", response.Buckets[1].Name);
                Assert.Equal("Bucket 3", response.Buckets[2].Name);
            }

            [Fact]
            public async Task When_GettingBucketsWithLimit_Expect_LimitedBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket { Name = "Bucket 1" });
                await this.repository.AddAsync(new Bucket { Name = "Bucket 2" });
                await this.repository.AddAsync(new Bucket { Name = "Bucket 3" });

                GetBuckets.Query query = new GetBuckets.Query { Limit = 2 };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default(CancellationToken));

                // Assert
                Assert.NotNull(response.Buckets);
                Assert.NotEmpty(response.Buckets);
                Assert.Equal(2, response.Buckets.Count);
                Assert.Equal("Bucket 1", response.Buckets[0].Name);
                Assert.Equal("Bucket 2", response.Buckets[1].Name);
            }

            [Fact]
            public async Task When_GettingBucketsWithNoOffset_Expect_AllBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket { Name = "Bucket 1" });
                await this.repository.AddAsync(new Bucket { Name = "Bucket 2" });
                await this.repository.AddAsync(new Bucket { Name = "Bucket 3" });

                GetBuckets.Query query = new GetBuckets.Query { Offset = null };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default(CancellationToken));

                // Assert
                Assert.NotNull(response.Buckets);
                Assert.NotEmpty(response.Buckets);
                Assert.Equal(3, response.Buckets.Count);
                Assert.Equal("Bucket 1", response.Buckets[0].Name);
                Assert.Equal("Bucket 2", response.Buckets[1].Name);
                Assert.Equal("Bucket 3", response.Buckets[2].Name);
            }

            [Fact]
            public async Task When_GettingBucketsWithOffset_Expect_OffsetBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket { Name = "Bucket 1" });
                await this.repository.AddAsync(new Bucket { Name = "Bucket 2" });
                await this.repository.AddAsync(new Bucket { Name = "Bucket 3" });

                GetBuckets.Query query = new GetBuckets.Query { Offset = 1 };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default(CancellationToken));

                // Assert
                Assert.NotNull(response.Buckets);
                Assert.NotEmpty(response.Buckets);
                Assert.Equal(2, response.Buckets.Count);
                Assert.Equal("Bucket 2", response.Buckets[0].Name);
                Assert.Equal("Bucket 3", response.Buckets[1].Name);
            }

            [Fact]
            public async Task When_GettingBucketsWithLimitAndOffset_Expect_LimitedAndOffsetBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket { Name = "Bucket 1" });
                await this.repository.AddAsync(new Bucket { Name = "Bucket 2" });
                await this.repository.AddAsync(new Bucket { Name = "Bucket 3" });

                GetBuckets.Query query = new GetBuckets.Query { Limit = 1, Offset = 1 };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default(CancellationToken));

                // Assert
                Assert.NotNull(response.Buckets);
                Assert.NotEmpty(response.Buckets);
                Assert.Equal(1, response.Buckets.Count);
                Assert.Equal("Bucket 2", response.Buckets[0].Name);
            }
        }
    }
}
