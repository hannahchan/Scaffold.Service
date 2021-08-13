namespace Scaffold.Application.UnitTests.Components.Bucket
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Common.Messaging;
    using Scaffold.Application.Components.Bucket;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Repositories;
    using Xunit;

    public class RemoveBucketUnitTests
    {
        private readonly IBucketRepository repository;

        private readonly Mock.Publisher publisher;

        public RemoveBucketUnitTests()
        {
            BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            this.repository = new ScopedBucketRepository(context);
            this.publisher = new Mock.Publisher();
        }

        public class Handler : RemoveBucketUnitTests
        {
            [Fact]
            public async Task When_RemovingBucket_Expect_Removed()
            {
                // Arrange
                Bucket bucket = new Bucket();
                await this.repository.AddAsync(bucket);

                RemoveBucket.Command command = new RemoveBucket.Command(bucket.Id);
                RemoveBucket.Handler handler = new RemoveBucket.Handler(this.repository, this.publisher);

                // Act
                await handler.Handle(command, default);

                // Assert
                Assert.Null(this.repository.Get(bucket.Id));

                Assert.Collection(
                    this.publisher.PublishedEvents,
                    publishedEvent =>
                    {
                        BucketRemovedEvent<RemoveBucket.Handler> bucketEvent = Assert.IsType<BucketRemovedEvent<RemoveBucket.Handler>>(publishedEvent.Notification);
                        Assert.Equal("BucketRemoved", bucketEvent.Type);
                        Assert.Equal($"Removed Bucket {bucket.Id}", bucketEvent.Description);
                        Assert.Equal(bucket.Id, bucketEvent.BucketId);
                        Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                    });
            }

            [Fact]
            public async Task When_RemovingNonExistingBucket_Expect_BucketNotFoundException()
            {
                // Arrange
                RemoveBucket.Command command = new RemoveBucket.Command(new Random().Next());
                RemoveBucket.Handler handler = new RemoveBucket.Handler(this.repository, this.publisher);

                // Act
                Exception exception = await Record.ExceptionAsync(() => handler.Handle(command, default));

                // Assert
                Assert.IsType<BucketNotFoundException>(exception);
                Assert.Empty(this.publisher.PublishedEvents);
            }
        }
    }
}
