namespace Scaffold.Application.UnitTests.Components.Bucket;

using System;
using System.Threading;
using System.Threading.Tasks;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Aggregates.Bucket;
using Xunit;

public class RemoveBucketUnitTests
{
    private readonly Mock.Publisher publisher = new Mock.Publisher();

    public class Handler : RemoveBucketUnitTests
    {
        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_RemovingBucket_Expect_Removed(IBucketRepository repository)
        {
            // Arrange
            Bucket bucket = new Bucket();
            repository.Add(bucket);

            RemoveBucket.Command command = new RemoveBucket.Command(bucket.Id);
            RemoveBucket.Handler handler = new RemoveBucket.Handler(repository, this.publisher);

            // Act
            await handler.Handle(command, default);

            // Assert
            Assert.Null(repository.Get(bucket.Id));

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketRemovedEvent bucketEvent = Assert.IsType<BucketRemovedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketRemoved", bucketEvent.Type);
                    Assert.Equal($"Removed Bucket {bucket.Id}", bucketEvent.Description);
                    Assert.Equal(bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_RemovingNonExistingBucket_Expect_BucketNotFoundException(IBucketRepository repository)
        {
            // Arrange
            RemoveBucket.Command command = new RemoveBucket.Command(new Random().Next());
            RemoveBucket.Handler handler = new RemoveBucket.Handler(repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() => handler.Handle(command, default));

            // Assert
            Assert.IsType<BucketNotFoundException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }
    }
}
