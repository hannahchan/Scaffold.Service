namespace Scaffold.Application.UnitTests.Components.Bucket;

using System;
using System.Threading;
using System.Threading.Tasks;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Aggregates.Bucket;
using Xunit;

public class RemoveItemUnitTests
{
    private readonly Mock.Publisher publisher = new Mock.Publisher();

    public class Handler : RemoveItemUnitTests
    {
        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_RemovingItemFromBucket_Expect_Removed(IBucketRepository repository)
        {
            // Arrange
            Bucket bucket = new Bucket();
            Item item = new Item();
            bucket.AddItem(item);
            repository.Add(bucket);

            RemoveItem.Command command = new RemoveItem.Command(
                BucketId: bucket.Id,
                ItemId: item.Id);

            RemoveItem.Handler handler = new RemoveItem.Handler(repository, this.publisher);

            // Act
            await handler.Handle(command, default);

            // Assert
            Assert.DoesNotContain(item, repository.Get(bucket.Id).Items);

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    ItemRemovedEvent bucketEvent = Assert.IsType<ItemRemovedEvent>(publishedEvent.Notification);
                    Assert.Equal("ItemRemoved", bucketEvent.Type);
                    Assert.Equal($"Removed Item {item.Id} from Bucket {bucket.Id}", bucketEvent.Description);
                    Assert.Equal(bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(item.Id, bucketEvent.ItemId);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_RemovingNonExistingItemFromBucket_Expect_ItemNotFoundException(IBucketRepository repository)
        {
            // Arrange
            Bucket bucket = new Bucket();
            Item item = new Item();
            bucket.AddItem(item);
            repository.Add(bucket);

            RemoveItem.Command command = new RemoveItem.Command(
                BucketId: bucket.Id,
                ItemId: new Random().Next());

            RemoveItem.Handler handler = new RemoveItem.Handler(repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() => handler.Handle(command, default));

            // Assert
            Assert.IsType<ItemNotFoundException>(exception);
            Assert.NotEmpty(repository.Get(bucket.Id).Items);
            Assert.Empty(this.publisher.PublishedEvents);
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_RemovingItemFromNonExistingBucket_Expect_BucketNotFoundException(IBucketRepository repository)
        {
            // Arrange
            Bucket bucket = new Bucket();
            Item item = new Item();
            bucket.AddItem(item);
            repository.Add(bucket);

            RemoveItem.Command command = new RemoveItem.Command(
                BucketId: new Random().Next(),
                ItemId: item.Id);

            RemoveItem.Handler handler = new RemoveItem.Handler(repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() => handler.Handle(command, default));

            // Assert
            Assert.IsType<BucketNotFoundException>(exception);
            Assert.NotEmpty(repository.Get(bucket.Id).Items);
            Assert.Empty(this.publisher.PublishedEvents);
        }
    }
}
