namespace Scaffold.Application.UnitTests.Components.Bucket
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Common.Messaging;
    using Scaffold.Application.Components.Bucket;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Repositories;
    using Xunit;

    public class RemoveItemUnitTests
    {
        private readonly IBucketRepository repository;

        private readonly Mock.Publisher publisher;

        public RemoveItemUnitTests()
        {
            BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            this.repository = new BucketRepository(context);
            this.publisher = new Mock.Publisher();
        }

        public class Handler : RemoveItemUnitTests
        {
            [Fact]
            public async Task When_RemovingItemFromBucket_Expect_Removed()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item();
                bucket.AddItem(item);
                await this.repository.AddAsync(bucket);

                RemoveItem.Command command = new RemoveItem.Command(
                    BucketId: bucket.Id,
                    ItemId: item.Id);

                RemoveItem.Handler handler = new RemoveItem.Handler(this.repository, this.publisher);

                // Act
                await handler.Handle(command, default);

                // Assert
                Assert.DoesNotContain(item, this.repository.Get(bucket.Id).Items);

                Assert.Collection(
                    this.publisher.PublishedEvents,
                    publishedEvent =>
                    {
                        ItemRemovedEvent<RemoveItem.Handler> bucketEvent = Assert.IsType<ItemRemovedEvent<RemoveItem.Handler>>(publishedEvent.Notification);
                        Assert.Equal("ItemRemoved", bucketEvent.Type);
                        Assert.Equal($"Removed Item {item.Id} from Bucket {bucket.Id}", bucketEvent.Description);
                        Assert.Equal(bucket.Id, bucketEvent.BucketId);
                        Assert.Equal(item.Id, bucketEvent.ItemId);
                        Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                    });
            }

            [Fact]
            public async Task When_RemovingNonExistingItemFromBucket_Expect_ItemNotFoundException()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item();
                bucket.AddItem(item);
                await this.repository.AddAsync(bucket);

                RemoveItem.Command command = new RemoveItem.Command(
                    BucketId: bucket.Id,
                    ItemId: new Random().Next());

                RemoveItem.Handler handler = new RemoveItem.Handler(this.repository, this.publisher);

                // Act
                Exception exception = await Record.ExceptionAsync(() => handler.Handle(command, default));

                // Assert
                Assert.IsType<ItemNotFoundException>(exception);
                Assert.NotEmpty(this.repository.Get(bucket.Id).Items);
                Assert.Empty(this.publisher.PublishedEvents);
            }

            [Fact]
            public async Task When_RemovingItemFromNonExistingBucket_Expect_BucketNotFoundException()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item();
                bucket.AddItem(item);
                await this.repository.AddAsync(bucket);

                RemoveItem.Command command = new RemoveItem.Command(
                    BucketId: new Random().Next(),
                    ItemId: item.Id);

                RemoveItem.Handler handler = new RemoveItem.Handler(this.repository, this.publisher);

                // Act
                Exception exception = await Record.ExceptionAsync(() => handler.Handle(command, default));

                // Assert
                Assert.IsType<BucketNotFoundException>(exception);
                Assert.NotEmpty(this.repository.Get(bucket.Id).Items);
                Assert.Empty(this.publisher.PublishedEvents);
            }
        }
    }
}
