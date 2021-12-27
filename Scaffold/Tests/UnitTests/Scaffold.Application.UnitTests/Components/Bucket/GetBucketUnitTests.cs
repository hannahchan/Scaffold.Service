namespace Scaffold.Application.UnitTests.Components.Bucket;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Aggregates.Bucket;
using Scaffold.Repositories;
using Xunit;

public class GetBucketUnitTests
{
    private readonly IBucketRepository repository;

    private readonly Mock.Publisher publisher;

    public GetBucketUnitTests()
    {
        BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        this.repository = new ScopedBucketRepository(context);
        this.publisher = new Mock.Publisher();
    }

    public class Handler : GetBucketUnitTests
    {
        [Fact]
        public async Task When_GettingBucket_Expect_ExistingBucket()
        {
            // Arrange
            Bucket bucket = new Bucket();
            await this.repository.AddAsync(bucket);

            GetBucket.Query query = new GetBucket.Query(bucket.Id);
            GetBucket.Handler handler = new GetBucket.Handler(this.repository, this.publisher);

            // Act
            GetBucket.Response response = await handler.Handle(query, default);

            // Assert
            Assert.Equal(bucket.Id, response.Bucket.Id);
            Assert.Equal(bucket.Name, response.Bucket.Name);

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketRetrievedEvent bucketEvent = Assert.IsType<BucketRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved Bucket {response.Bucket.Id}", bucketEvent.Description);
                    Assert.Equal(response.Bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Fact]
        public async Task When_GettingNonExistingBucket_Expect_BucketNotFoundException()
        {
            // Arrange
            GetBucket.Query query = new GetBucket.Query(new Random().Next());
            GetBucket.Handler handler = new GetBucket.Handler(this.repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() =>
                handler.Handle(query, default));

            // Assert
            Assert.IsType<BucketNotFoundException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }
    }
}
