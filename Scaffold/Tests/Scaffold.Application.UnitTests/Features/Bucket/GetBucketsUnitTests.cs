namespace Scaffold.Application.UnitTests.Features.Bucket
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.Application.Interfaces;
    using Scaffold.Application.Models;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Repositories.PostgreSQL;
    using Xunit;

    public class GetBucketsUnitTests
    {
        private readonly IBucketRepository repository;

        private readonly IList<Bucket> testBuckets = new List<Bucket>();

        public GetBucketsUnitTests()
        {
            BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            this.repository = new BucketRepository(context);

            this.testBuckets.Add(new Bucket { Name = "B", Description = "1", Size = 3 });
            this.testBuckets.Add(new Bucket { Name = "A", Description = "3", Size = 9 });
            this.testBuckets.Add(new Bucket { Name = "B", Description = "1", Size = 7 });
            this.testBuckets.Add(new Bucket { Name = "B", Description = "3", Size = 10 });
            this.testBuckets.Add(new Bucket { Name = "B", Description = "3", Size = 4 });
            this.testBuckets.Add(new Bucket { Name = "A", Description = "3", Size = 6 });
            this.testBuckets.Add(new Bucket { Name = "B", Description = "2", Size = 1 });
            this.testBuckets.Add(new Bucket { Name = "A", Description = "2", Size = 11 });
            this.testBuckets.Add(new Bucket { Name = "A", Description = "1", Size = 5 });
            this.testBuckets.Add(new Bucket { Name = "A", Description = "2", Size = 12 });
            this.testBuckets.Add(new Bucket { Name = "B", Description = "2", Size = 2 });
            this.testBuckets.Add(new Bucket { Name = "A", Description = "1", Size = 8 });
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
                Assert.Null(query.Ordering);
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
                Assert.Null(query.Ordering);
            }

            [Fact]
            public void When_InstantiatingQueryWithNullPredicate_Expect_ArgumentNullException()
            {
                // Arrange
                Exception exception;

                // Act
                exception = Record.Exception(() => new GetBuckets.Query(null!));

                // Assert
                Assert.IsType<ArgumentNullException>(exception);
            }
        }

        public class Response
        {
            [Fact]
            public void When_InstantiatingResponseWithBuckets_Expect_ResponseWithBuckets()
            {
                // Arrange
                List<Bucket> buckets = new List<Bucket>();

                // Act
                GetBuckets.Response response = new GetBuckets.Response(buckets);

                // Assert
                Assert.Equal(buckets, response.Buckets);
            }

            [Fact]
            public void When_InstantiatingResponseWithNull_Expect_ArgumentNullException()
            {
                // Act
                Exception exception = Record.Exception(() => new GetBuckets.Response(null!));

                // Assert
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
                GetBuckets.Response response = await handler.Handle(query, default);

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
                GetBuckets.Response response = await handler.Handle(query, default);

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
                GetBuckets.Response response = await handler.Handle(query, default);

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
                GetBuckets.Response response = await handler.Handle(query, default);

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
                GetBuckets.Response response = await handler.Handle(query, default);

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
                GetBuckets.Response response = await handler.Handle(query, default);

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
                GetBuckets.Response response = await handler.Handle(query, default);

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
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.NotNull(response.Buckets);
                Assert.NotEmpty(response.Buckets);
                Assert.Equal(1, response.Buckets.Count);
                Assert.Equal("Bucket 2", response.Buckets[0].Name);
            }

            [Fact]
            public async Task When_GettingBucketsOrderedBySizeAscending_Expect_OrderedBySizeAscending()
            {
                // Arrange
                foreach (Bucket bucket in this.testBuckets)
                {
                    await this.repository.AddAsync(bucket);
                }

                Ordering<Bucket> ordering = new Ordering<Bucket> { new OrderBy("Size", true) };

                GetBuckets.Query query = new GetBuckets.Query { Ordering = ordering };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                for (int i = 1; i <= 12; i++)
                {
                    Assert.Equal(i, response.Buckets[i - 1].Size);
                }
            }

            [Fact]
            public async Task When_GettingBucketsOrderedBySizeDescending_Expect_OrderedBySizeDescending()
            {
                // Arrange
                foreach (Bucket bucket in this.testBuckets)
                {
                    await this.repository.AddAsync(bucket);
                }

                Ordering<Bucket> ordering = new Ordering<Bucket> { new OrderBy("Size", false) };

                GetBuckets.Query query = new GetBuckets.Query { Ordering = ordering };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                for (int i = 1; i <= 12; i++)
                {
                    Assert.Equal(12 - (i - 1), response.Buckets[i - 1].Size);
                }
            }

            [Fact]
            public async Task When_GettingBucketsOrderedBySizeWithLimit_Expect_OrderedLimitedBuckets()
            {
                // Arrange
                foreach (Bucket bucket in this.testBuckets)
                {
                    await this.repository.AddAsync(bucket);
                }

                Ordering<Bucket> ordering = new Ordering<Bucket> { new OrderBy("Size", true) };

                GetBuckets.Query query = new GetBuckets.Query { Limit = 6, Ordering = ordering };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                for (int i = 1; i <= 6; i++)
                {
                    Assert.Equal(i, response.Buckets[i - 1].Size);
                }
            }

            [Fact]
            public async Task When_GettingBucketsOrderedBySizeWithOffset_Expect_OrderedOffsetBuckets()
            {
                // Arrange
                foreach (Bucket bucket in this.testBuckets)
                {
                    await this.repository.AddAsync(bucket);
                }

                Ordering<Bucket> ordering = new Ordering<Bucket> { new OrderBy("Size", true) };

                GetBuckets.Query query = new GetBuckets.Query { Offset = 6, Ordering = ordering };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                for (int i = 1; i <= 6; i++)
                {
                    Assert.Equal(i + 6, response.Buckets[i - 1].Size);
                }
            }

            [Fact]
            public async Task When_GettingBucketsOrderedBySizeWithLimtAndOffset_Expect_OrderedLimitedAndOffsetBuckets()
            {
                // Arrange
                foreach (Bucket bucket in this.testBuckets)
                {
                    await this.repository.AddAsync(bucket);
                }

                Ordering<Bucket> ordering = new Ordering<Bucket> { new OrderBy("Size", true) };

                GetBuckets.Query query = new GetBuckets.Query { Limit = 6, Offset = 3, Ordering = ordering };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                for (int i = 1; i <= 6; i++)
                {
                    Assert.Equal(i + 3, response.Buckets[i - 1].Size);
                }
            }

            [Fact]
            public async Task When_GettingBucketsOrderedByNameAscendingThenByDescriptionAscending_Expect_OrderedByNameAscendingThenByDescriptionAscending()
            {
                // Arrange
                foreach (Bucket bucket in this.testBuckets)
                {
                    await this.repository.AddAsync(bucket);
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>
                {
                    new OrderBy("Name", true),
                    new OrderBy("Description", true),
                };

                GetBuckets.Query query = new GetBuckets.Query { Ordering = ordering };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Equal("A", response.Buckets[0].Name);
                Assert.Equal("A", response.Buckets[1].Name);
                Assert.Equal("A", response.Buckets[2].Name);
                Assert.Equal("A", response.Buckets[3].Name);
                Assert.Equal("A", response.Buckets[4].Name);
                Assert.Equal("A", response.Buckets[5].Name);
                Assert.Equal("B", response.Buckets[6].Name);
                Assert.Equal("B", response.Buckets[7].Name);
                Assert.Equal("B", response.Buckets[8].Name);
                Assert.Equal("B", response.Buckets[9].Name);
                Assert.Equal("B", response.Buckets[10].Name);
                Assert.Equal("B", response.Buckets[11].Name);

                Assert.Equal("1", response.Buckets[0].Description);
                Assert.Equal("1", response.Buckets[1].Description);
                Assert.Equal("2", response.Buckets[2].Description);
                Assert.Equal("2", response.Buckets[3].Description);
                Assert.Equal("3", response.Buckets[4].Description);
                Assert.Equal("3", response.Buckets[5].Description);
                Assert.Equal("1", response.Buckets[6].Description);
                Assert.Equal("1", response.Buckets[7].Description);
                Assert.Equal("2", response.Buckets[8].Description);
                Assert.Equal("2", response.Buckets[9].Description);
                Assert.Equal("3", response.Buckets[10].Description);
                Assert.Equal("3", response.Buckets[11].Description);
            }

            [Fact]
            public async Task When_GettingBucketsOrderedByNameDescendingThenByDescriptionDescending_Expect_OrderedByNameDescendingThenByDescriptionDescending()
            {
                // Arrange
                foreach (Bucket bucket in this.testBuckets)
                {
                    await this.repository.AddAsync(bucket);
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>
                {
                    new OrderBy("Name", false),
                    new OrderBy("Description", false),
                };

                GetBuckets.Query query = new GetBuckets.Query { Ordering = ordering };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Equal("B", response.Buckets[0].Name);
                Assert.Equal("B", response.Buckets[1].Name);
                Assert.Equal("B", response.Buckets[2].Name);
                Assert.Equal("B", response.Buckets[3].Name);
                Assert.Equal("B", response.Buckets[4].Name);
                Assert.Equal("B", response.Buckets[5].Name);
                Assert.Equal("A", response.Buckets[6].Name);
                Assert.Equal("A", response.Buckets[7].Name);
                Assert.Equal("A", response.Buckets[8].Name);
                Assert.Equal("A", response.Buckets[9].Name);
                Assert.Equal("A", response.Buckets[10].Name);
                Assert.Equal("A", response.Buckets[11].Name);

                Assert.Equal("3", response.Buckets[0].Description);
                Assert.Equal("3", response.Buckets[1].Description);
                Assert.Equal("2", response.Buckets[2].Description);
                Assert.Equal("2", response.Buckets[3].Description);
                Assert.Equal("1", response.Buckets[4].Description);
                Assert.Equal("1", response.Buckets[5].Description);
                Assert.Equal("3", response.Buckets[6].Description);
                Assert.Equal("3", response.Buckets[7].Description);
                Assert.Equal("2", response.Buckets[8].Description);
                Assert.Equal("2", response.Buckets[9].Description);
                Assert.Equal("1", response.Buckets[10].Description);
                Assert.Equal("1", response.Buckets[11].Description);
            }

            [Fact]
            public async Task When_GettingBucketsOrderedByNameAscendingThenByDescriptionDescending_Expect_OrderedByNameAscendingThenByDescriptionDescending()
            {
                // Arrange
                foreach (Bucket bucket in this.testBuckets)
                {
                    await this.repository.AddAsync(bucket);
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>
                {
                    new OrderBy("Name", true),
                    new OrderBy("Description", false),
                };

                GetBuckets.Query query = new GetBuckets.Query { Ordering = ordering };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Equal("A", response.Buckets[0].Name);
                Assert.Equal("A", response.Buckets[1].Name);
                Assert.Equal("A", response.Buckets[2].Name);
                Assert.Equal("A", response.Buckets[3].Name);
                Assert.Equal("A", response.Buckets[4].Name);
                Assert.Equal("A", response.Buckets[5].Name);
                Assert.Equal("B", response.Buckets[6].Name);
                Assert.Equal("B", response.Buckets[7].Name);
                Assert.Equal("B", response.Buckets[8].Name);
                Assert.Equal("B", response.Buckets[9].Name);
                Assert.Equal("B", response.Buckets[10].Name);
                Assert.Equal("B", response.Buckets[11].Name);

                Assert.Equal("3", response.Buckets[0].Description);
                Assert.Equal("3", response.Buckets[1].Description);
                Assert.Equal("2", response.Buckets[2].Description);
                Assert.Equal("2", response.Buckets[3].Description);
                Assert.Equal("1", response.Buckets[4].Description);
                Assert.Equal("1", response.Buckets[5].Description);
                Assert.Equal("3", response.Buckets[6].Description);
                Assert.Equal("3", response.Buckets[7].Description);
                Assert.Equal("2", response.Buckets[8].Description);
                Assert.Equal("2", response.Buckets[9].Description);
                Assert.Equal("1", response.Buckets[10].Description);
                Assert.Equal("1", response.Buckets[11].Description);
            }

            [Fact]
            public async Task When_GettingBucketsOrderedByNameDescendingThenByDescriptionAscending_Expect_OrderedByNameDescendingThenByDescriptionAscending()
            {
                // Arrange
                foreach (Bucket bucket in this.testBuckets)
                {
                    await this.repository.AddAsync(bucket);
                }

                Ordering<Bucket> ordering = new Ordering<Bucket>
                {
                    new OrderBy("Name", false),
                    new OrderBy("Description", true),
                };

                GetBuckets.Query query = new GetBuckets.Query { Ordering = ordering };
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Equal("B", response.Buckets[0].Name);
                Assert.Equal("B", response.Buckets[1].Name);
                Assert.Equal("B", response.Buckets[2].Name);
                Assert.Equal("B", response.Buckets[3].Name);
                Assert.Equal("B", response.Buckets[4].Name);
                Assert.Equal("B", response.Buckets[5].Name);
                Assert.Equal("A", response.Buckets[6].Name);
                Assert.Equal("A", response.Buckets[7].Name);
                Assert.Equal("A", response.Buckets[8].Name);
                Assert.Equal("A", response.Buckets[9].Name);
                Assert.Equal("A", response.Buckets[10].Name);
                Assert.Equal("A", response.Buckets[11].Name);

                Assert.Equal("1", response.Buckets[0].Description);
                Assert.Equal("1", response.Buckets[1].Description);
                Assert.Equal("2", response.Buckets[2].Description);
                Assert.Equal("2", response.Buckets[3].Description);
                Assert.Equal("3", response.Buckets[4].Description);
                Assert.Equal("3", response.Buckets[5].Description);
                Assert.Equal("1", response.Buckets[6].Description);
                Assert.Equal("1", response.Buckets[7].Description);
                Assert.Equal("2", response.Buckets[8].Description);
                Assert.Equal("2", response.Buckets[9].Description);
                Assert.Equal("3", response.Buckets[10].Description);
                Assert.Equal("3", response.Buckets[11].Description);
            }
        }
    }
}
