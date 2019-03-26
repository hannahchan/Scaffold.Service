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
        }
    }
}
