namespace Scaffold.Application.UnitTests.Features.Bucket
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Common.Models;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Repositories;
    using Xunit;

    public class GetBucketsUnitTests
    {
        private readonly IBucketRepository repository;

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
            public void When_InstantiatingQuery_Expect_PredicateNotNull()
            {
                // Arrange
                GetBuckets.Query query;

                // Act
                query = new GetBuckets.Query();

                // Assert
                Assert.NotNull(query.Predicate);
                Assert.Null(query.Limit);
                Assert.Null(query.Offset);
                Assert.Null(query.SortOrder);
            }

            [Fact]
            public void When_InstantiatingQueryWithPredicate_Expect_Predicate()
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
                Assert.Null(query.SortOrder);
            }

            [Fact]
            public void When_InstantiatingQueryWithNullPredicate_Expect_ArgumentNullException()
            {
                // Arrange
                Exception exception;

                // Act
                exception = Record.Exception(() => new GetBuckets.Query(null));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("predicate", argumentNullException.ParamName);
            }
        }

        public class Response
        {
            [Fact]
            public void When_InstantiatingResponseWithBuckets_Expect_ResponseWithBuckets()
            {
                // Arrange
                Bucket[] buckets = Array.Empty<Bucket>();

                // Act
                GetBuckets.Response response = new GetBuckets.Response(buckets);

                // Assert
                Assert.Equal(buckets, response.Buckets);
            }

            [Fact]
            public void When_InstantiatingResponseWithNull_Expect_ArgumentNullException()
            {
                // Act
                Exception exception = Record.Exception(() => new GetBuckets.Response(null));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("buckets", argumentNullException.ParamName);
            }
        }

        public class Handler : GetBucketsUnitTests
        {
            [Fact]
            public async Task When_GettingBuckets_Expect_AllBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                GetBuckets.Query query = new GetBuckets.Query();
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 2", bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name));
            }

            [Fact]
            public async Task When_GettingNonExistingBuckets_Expect_Empty()
            {
                // Arrange
                GetBuckets.Query query = new GetBuckets.Query();
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Empty(response.Buckets);
            }

            [Fact]
            public async Task When_GettingBucketsWithPredicate_Expect_SomeBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket[]
                {
                    new Bucket { Size = 1 },
                    new Bucket { Size = 2 },
                    new Bucket { Size = 3 },
                    new Bucket { Size = 5 },
                    new Bucket { Size = 8 },
                });

                GetBuckets.Query query = new GetBuckets.Query(bucket => bucket.Size == 2 || bucket.Size == 5);
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
                    bucket => Assert.Equal(2, bucket.Size),
                    bucket => Assert.Equal(5, bucket.Size));
            }

            [Fact]
            public async Task When_GettingBucketsWithNoLimitAndNoOffset_Expect_AllBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                GetBuckets.Query query = new GetBuckets.Query(
                    predicate: bucket => true,
                    limit: null,
                    offset: null,
                    sortOrder: null);

                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 2", bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name));
            }

            [Fact]
            public async Task When_GettingBucketsWithLimit_Expect_LimitedBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                GetBuckets.Query query = new GetBuckets.Query(
                    predicate: bucket => true,
                    limit: 2,
                    offset: null,
                    sortOrder: null);

                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
                    bucket => Assert.Equal("Bucket 1", bucket.Name),
                    bucket => Assert.Equal("Bucket 2", bucket.Name));
            }

            [Fact]
            public async Task When_GettingBucketsWithOffset_Expect_OffsetBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                GetBuckets.Query query = new GetBuckets.Query(
                    predicate: bucket => true,
                    limit: null,
                    offset: 1,
                    sortOrder: null);

                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
                    bucket => Assert.Equal("Bucket 2", bucket.Name),
                    bucket => Assert.Equal("Bucket 3", bucket.Name));
            }

            [Fact]
            public async Task When_GettingBucketsWithLimitAndOffset_Expect_LimitedAndOffsetBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket[]
                {
                    new Bucket { Name = "Bucket 1" },
                    new Bucket { Name = "Bucket 2" },
                    new Bucket { Name = "Bucket 3" },
                });

                GetBuckets.Query query = new GetBuckets.Query(
                    predicate: bucket => true,
                    limit: 1,
                    offset: 1,
                    sortOrder: null);

                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(response.Buckets, bucket => Assert.Equal("Bucket 2", bucket.Name));
            }

            [Fact]
            public async Task When_GettingBucketsOrderedBySizeAscending_Expect_OrderedBySizeAscending()
            {
                // Arrange
                await this.repository.AddAsync(this.testBuckets);

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Size);

                GetBuckets.Query query = new GetBuckets.Query(
                    predicate: bucket => true,
                    limit: null,
                    offset: null,
                    sortOrder: sortOrder);

                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
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
                await this.repository.AddAsync(this.testBuckets);

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderByDescending(bucket => bucket.Size);

                GetBuckets.Query query = new GetBuckets.Query(
                    predicate: bucket => true,
                    limit: null,
                    offset: null,
                    sortOrder: sortOrder);

                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
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
                await this.repository.AddAsync(this.testBuckets);

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Size);

                GetBuckets.Query query = new GetBuckets.Query(
                    predicate: bucket => true,
                    limit: 6,
                    offset: null,
                    sortOrder: sortOrder);

                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
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
                await this.repository.AddAsync(this.testBuckets);

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Size);

                GetBuckets.Query query = new GetBuckets.Query(
                    predicate: bucket => true,
                    limit: null,
                    offset: 6,
                    sortOrder: sortOrder);

                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
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
                await this.repository.AddAsync(this.testBuckets);

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Size);

                GetBuckets.Query query = new GetBuckets.Query(
                    predicate: bucket => true,
                    limit: 6,
                    offset: 3,
                    sortOrder: sortOrder);

                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
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
                await this.repository.AddAsync(this.testBuckets);

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Name)
                    .ThenBy(bucket => bucket.Description);

                GetBuckets.Query query = new GetBuckets.Query(
                    predicate: bucket => true,
                    limit: null,
                    offset: null,
                    sortOrder: sortOrder);

                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
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
                await this.repository.AddAsync(this.testBuckets);

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderByDescending(bucket => bucket.Name)
                    .ThenByDescending(bucket => bucket.Description);

                GetBuckets.Query query = new GetBuckets.Query(
                    predicate: bucket => true,
                    limit: null,
                    offset: null,
                    sortOrder: sortOrder);

                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
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
                await this.repository.AddAsync(this.testBuckets);

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderBy(bucket => bucket.Name)
                    .ThenByDescending(bucket => bucket.Description);

                GetBuckets.Query query = new GetBuckets.Query(
                    predicate: bucket => true,
                    limit: null,
                    offset: null,
                    sortOrder: sortOrder);

                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
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
                await this.repository.AddAsync(this.testBuckets);

                SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                    .OrderByDescending(bucket => bucket.Name)
                    .ThenBy(bucket => bucket.Description);

                GetBuckets.Query query = new GetBuckets.Query(
                    predicate: bucket => true,
                    limit: null,
                    offset: null,
                    sortOrder: sortOrder);

                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Collection(
                    response.Buckets,
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
