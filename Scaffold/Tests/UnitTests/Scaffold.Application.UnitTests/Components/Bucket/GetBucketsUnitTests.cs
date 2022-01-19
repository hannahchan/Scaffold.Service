namespace Scaffold.Application.UnitTests.Components.Bucket;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Common.Models;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Aggregates.Bucket;
using Xunit;

public class GetBucketsUnitTests
{
    private readonly Mock.Publisher publisher = new Mock.Publisher();

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

    public class Handler : GetBucketsUnitTests
    {
        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBuckets_Expect_AllBuckets(IBucketRepository repository)
        {
            // Arrange
            repository.Add(new Bucket[]
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
            });

            GetBuckets.Query query = new GetBuckets.Query(bucket => true);
            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

            // Act
            GetBuckets.Response response = await handler.Handle(query, default);

            // Assert
            Assert.Collection(
                response.Buckets,
                bucket => Assert.Equal("Bucket 1", bucket.Name),
                bucket => Assert.Equal("Bucket 2", bucket.Name),
                bucket => Assert.Equal("Bucket 3", bucket.Name));

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingNonExistingBuckets_Expect_Empty(IBucketRepository repository)
        {
            // Arrange
            GetBuckets.Query query = new GetBuckets.Query(bucket => true);
            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

            // Act
            GetBuckets.Response response = await handler.Handle(query, default);

            // Assert
            Assert.Empty(response.Buckets);

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsWithPredicate_Expect_SomeBuckets(IBucketRepository repository)
        {
            // Arrange
            repository.Add(new Bucket[]
            {
                new Bucket { Size = 1 },
                new Bucket { Size = 2 },
                new Bucket { Size = 3 },
                new Bucket { Size = 5 },
                new Bucket { Size = 8 },
            });

            GetBuckets.Query query = new GetBuckets.Query(bucket => bucket.Size == 2 || bucket.Size == 5);
            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

            // Act
            GetBuckets.Response response = await handler.Handle(query, default);

            // Assert
            Assert.Collection(
                response.Buckets,
                bucket => Assert.Equal(2, bucket.Size),
                bucket => Assert.Equal(5, bucket.Size));

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsWithNoLimitAndNoOffset_Expect_AllBuckets(IBucketRepository repository)
        {
            // Arrange
            repository.Add(new Bucket[]
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
            });

            GetBuckets.Query query = new GetBuckets.Query(
                Predicate: bucket => true,
                Limit: null,
                Offset: null,
                SortOrder: null);

            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

            // Act
            GetBuckets.Response response = await handler.Handle(query, default);

            // Assert
            Assert.Collection(
                response.Buckets,
                bucket => Assert.Equal("Bucket 1", bucket.Name),
                bucket => Assert.Equal("Bucket 2", bucket.Name),
                bucket => Assert.Equal("Bucket 3", bucket.Name));

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsWithLimit_Expect_LimitedBuckets(IBucketRepository repository)
        {
            // Arrange
            repository.Add(new Bucket[]
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
            });

            GetBuckets.Query query = new GetBuckets.Query(
                Predicate: bucket => true,
                Limit: 2,
                Offset: null,
                SortOrder: null);

            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

            // Act
            GetBuckets.Response response = await handler.Handle(query, default);

            // Assert
            Assert.Collection(
                response.Buckets,
                bucket => Assert.Equal("Bucket 1", bucket.Name),
                bucket => Assert.Equal("Bucket 2", bucket.Name));

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsWithOffset_Expect_OffsetBuckets(IBucketRepository repository)
        {
            // Arrange
            repository.Add(new Bucket[]
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
            });

            GetBuckets.Query query = new GetBuckets.Query(
                Predicate: bucket => true,
                Limit: null,
                Offset: 1,
                SortOrder: null);

            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

            // Act
            GetBuckets.Response response = await handler.Handle(query, default);

            // Assert
            Assert.Collection(
                response.Buckets,
                bucket => Assert.Equal("Bucket 2", bucket.Name),
                bucket => Assert.Equal("Bucket 3", bucket.Name));

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsWithLimitAndOffset_Expect_LimitedAndOffsetBuckets(IBucketRepository repository)
        {
            // Arrange
            repository.Add(new Bucket[]
            {
                new Bucket { Name = "Bucket 1" },
                new Bucket { Name = "Bucket 2" },
                new Bucket { Name = "Bucket 3" },
            });

            GetBuckets.Query query = new GetBuckets.Query(
                Predicate: bucket => true,
                Limit: 1,
                Offset: 1,
                SortOrder: null);

            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

            // Act
            GetBuckets.Response response = await handler.Handle(query, default);

            // Assert
            Assert.Collection(response.Buckets, bucket => Assert.Equal("Bucket 2", bucket.Name));

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsOrderedBySizeAscending_Expect_OrderedBySizeAscending(IBucketRepository repository)
        {
            // Arrange
            repository.Add(this.testBuckets);

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Size);

            GetBuckets.Query query = new GetBuckets.Query(
                Predicate: bucket => true,
                Limit: null,
                Offset: null,
                SortOrder: sortOrder);

            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

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

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsOrderedBySizeDescending_Expect_OrderedBySizeDescending(IBucketRepository repository)
        {
            // Arrange
            repository.Add(this.testBuckets);

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderByDescending(bucket => bucket.Size);

            GetBuckets.Query query = new GetBuckets.Query(
                Predicate: bucket => true,
                Limit: null,
                Offset: null,
                SortOrder: sortOrder);

            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

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

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsOrderedBySizeWithLimit_Expect_OrderedLimitedBuckets(IBucketRepository repository)
        {
            // Arrange
            repository.Add(this.testBuckets);

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Size);

            GetBuckets.Query query = new GetBuckets.Query(
                Predicate: bucket => true,
                Limit: 6,
                Offset: null,
                SortOrder: sortOrder);

            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

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

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsOrderedBySizeWithOffset_Expect_OrderedOffsetBuckets(IBucketRepository repository)
        {
            // Arrange
            repository.Add(this.testBuckets);

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Size);

            GetBuckets.Query query = new GetBuckets.Query(
                Predicate: bucket => true,
                Limit: null,
                Offset: 6,
                SortOrder: sortOrder);

            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

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

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsOrderedBySizeWithLimtAndOffset_Expect_OrderedLimitedAndOffsetBuckets(IBucketRepository repository)
        {
            // Arrange
            repository.Add(this.testBuckets);

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Size);

            GetBuckets.Query query = new GetBuckets.Query(
                Predicate: bucket => true,
                Limit: 6,
                Offset: 3,
                SortOrder: sortOrder);

            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

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

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsOrderedByNameAscendingThenByDescriptionAscending_Expect_OrderedByNameAscendingThenByDescriptionAscending(IBucketRepository repository)
        {
            // Arrange
            repository.Add(this.testBuckets);

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Name)
                .ThenBy(bucket => bucket.Description);

            GetBuckets.Query query = new GetBuckets.Query(
                Predicate: bucket => true,
                Limit: null,
                Offset: null,
                SortOrder: sortOrder);

            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

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

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsOrderedByNameDescendingThenByDescriptionDescending_Expect_OrderedByNameDescendingThenByDescriptionDescending(IBucketRepository repository)
        {
            // Arrange
            repository.Add(this.testBuckets);

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderByDescending(bucket => bucket.Name)
                .ThenByDescending(bucket => bucket.Description);

            GetBuckets.Query query = new GetBuckets.Query(
                Predicate: bucket => true,
                Limit: null,
                Offset: null,
                SortOrder: sortOrder);

            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

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

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsOrderedByNameAscendingThenByDescriptionDescending_Expect_OrderedByNameAscendingThenByDescriptionDescending(IBucketRepository repository)
        {
            // Arrange
            repository.Add(this.testBuckets);

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderBy(bucket => bucket.Name)
                .ThenByDescending(bucket => bucket.Description);

            GetBuckets.Query query = new GetBuckets.Query(
                Predicate: bucket => true,
                Limit: null,
                Offset: null,
                SortOrder: sortOrder);

            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

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

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingBucketsOrderedByNameDescendingThenByDescriptionAscending_Expect_OrderedByNameDescendingThenByDescriptionAscending(IBucketRepository repository)
        {
            // Arrange
            repository.Add(this.testBuckets);

            SortOrder<Bucket> sortOrder = SortOrder<Bucket>
                .OrderByDescending(bucket => bucket.Name)
                .ThenBy(bucket => bucket.Description);

            GetBuckets.Query query = new GetBuckets.Query(
                Predicate: bucket => true,
                Limit: null,
                Offset: null,
                SortOrder: sortOrder);

            GetBuckets.Handler handler = new GetBuckets.Handler(repository, this.publisher);

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

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketsRetrievedEvent bucketEvent = Assert.IsType<BucketsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Buckets.Count()} Bucket/s", bucketEvent.Description);
                    Assert.Equal(response.Buckets.Select(bucket => bucket.Id).ToArray(), bucketEvent.BucketIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }
    }
}
