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

public class GetItemUnitTests
{
    private readonly IBucketRepository repository;

    private readonly Mock.Publisher publisher;

    public GetItemUnitTests()
    {
        BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        this.repository = new ScopedBucketRepository(context);
        this.publisher = new Mock.Publisher();
    }

    public class Handler : GetItemUnitTests
    {
        [Fact]
        public async Task When_GettingItemFromBucket_Expect_ExistingItem()
        {
            // Arrange
            Bucket bucket = new Bucket();
            Item item = new Item();
            bucket.AddItem(item);
            await this.repository.AddAsync(bucket);

            GetItem.Query query = new GetItem.Query(
                BucketId: bucket.Id,
                ItemId: item.Id);

            GetItem.Handler handler = new GetItem.Handler(this.repository, this.publisher);

            // Act
            GetItem.Response response = await handler.Handle(query, default);

            // Assert
            Assert.Equal(item.Id, response.Item.Id);
            Assert.Equal(item.Name, response.Item.Name);

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    ItemRetrievedEvent<GetItem.Handler> bucketEvent = Assert.IsType<ItemRetrievedEvent<GetItem.Handler>>(publishedEvent.Notification);
                    Assert.Equal("ItemRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved Item {item.Id} from Bucket {bucket.Id}", bucketEvent.Description);
                    Assert.Equal(bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(item.Id, bucketEvent.ItemId);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Fact]
        public async Task When_GettingNonExistingItemFromBucket_Expect_ItemNotFoundException()
        {
            // Arrange
            Bucket bucket = new Bucket();
            await this.repository.AddAsync(bucket);

            GetItem.Query query = new GetItem.Query(
                BucketId: bucket.Id,
                ItemId: new Random().Next());

            GetItem.Handler handler = new GetItem.Handler(this.repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() =>
                handler.Handle(query, default));

            // Assert
            Assert.IsType<ItemNotFoundException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }

        [Fact]
        public async Task When_GettingItemFromNonExistingBucket_Expect_BucketNotFoundException()
        {
            // Arrange
            GetItem.Query query = new GetItem.Query(
                BucketId: new Random().Next(),
                ItemId: new Random().Next());

            GetItem.Handler handler = new GetItem.Handler(this.repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() =>
                handler.Handle(query, default));

            // Assert
            Assert.IsType<BucketNotFoundException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }
    }
}
