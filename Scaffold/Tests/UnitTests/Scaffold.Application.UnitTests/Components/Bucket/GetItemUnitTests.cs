namespace Scaffold.Application.UnitTests.Components.Bucket;

using System;
using System.Threading;
using System.Threading.Tasks;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Aggregates.Bucket;
using Xunit;

public class GetItemUnitTests
{
    private readonly Mock.Publisher publisher = new Mock.Publisher();

    public class Handler : GetItemUnitTests
    {
        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingItemFromBucket_Expect_ExistingItem(IBucketRepository repository)
        {
            // Arrange
            Bucket bucket = new Bucket();
            Item item = new Item();
            bucket.AddItem(item);
            repository.Add(bucket);

            GetItem.Query query = new GetItem.Query(
                BucketId: bucket.Id,
                ItemId: item.Id);

            GetItem.Handler handler = new GetItem.Handler(repository, this.publisher);

            // Act
            GetItem.Response response = await handler.Handle(query, default);

            // Assert
            Assert.Equal(item.Id, response.Item.Id);
            Assert.Equal(item.Name, response.Item.Name);

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    ItemRetrievedEvent bucketEvent = Assert.IsType<ItemRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("ItemRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved Item {item.Id} from Bucket {bucket.Id}", bucketEvent.Description);
                    Assert.Equal(bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(item.Id, bucketEvent.ItemId);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingNonExistingItemFromBucket_Expect_ItemNotFoundException(IBucketRepository repository)
        {
            // Arrange
            Bucket bucket = new Bucket();
            repository.Add(bucket);

            GetItem.Query query = new GetItem.Query(
                BucketId: bucket.Id,
                ItemId: new Random().Next());

            GetItem.Handler handler = new GetItem.Handler(repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() =>
                handler.Handle(query, default));

            // Assert
            Assert.IsType<ItemNotFoundException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingItemFromNonExistingBucket_Expect_BucketNotFoundException(IBucketRepository repository)
        {
            // Arrange
            GetItem.Query query = new GetItem.Query(
                BucketId: new Random().Next(),
                ItemId: new Random().Next());

            GetItem.Handler handler = new GetItem.Handler(repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() =>
                handler.Handle(query, default));

            // Assert
            Assert.IsType<BucketNotFoundException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }
    }
}
