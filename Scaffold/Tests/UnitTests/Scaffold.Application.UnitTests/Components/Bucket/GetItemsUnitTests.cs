namespace Scaffold.Application.UnitTests.Components.Bucket;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Aggregates.Bucket;
using Xunit;

public class GetItemsUnitTests
{
    private readonly Mock.Publisher publisher = new Mock.Publisher();

    public class Handler : GetItemsUnitTests
    {
        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingItemsFromBucket_Expect_ExistingItems(IBucketRepository repository)
        {
            // Arrange
            Bucket bucket = new Bucket();
            bucket.AddItem(new Item { Name = "Item 1" });
            bucket.AddItem(new Item { Name = "Item 2" });
            bucket.AddItem(new Item { Name = "Item 3" });

            repository.Add(bucket);

            GetItems.Query query = new GetItems.Query(bucket.Id);
            GetItems.Handler handler = new GetItems.Handler(repository, this.publisher);

            // Act
            GetItems.Response response = await handler.Handle(query, default);

            // Assert
            Assert.Collection(
                response.Items,
                item => Assert.Equal("Item 1", item.Name),
                item => Assert.Equal("Item 2", item.Name),
                item => Assert.Equal("Item 3", item.Name));

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    ItemsRetrievedEvent bucketEvent = Assert.IsType<ItemsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("ItemsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Items.Count()} Item/s from Bucket {bucket.Id}", bucketEvent.Description);
                    Assert.Equal(bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(response.Items.Select(item => item.Id).ToArray(), bucketEvent.ItemIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingNonExistingItemsFromBucket_Expect_Empty(IBucketRepository repository)
        {
            // Arrange
            Bucket bucket = new Bucket();
            repository.Add(bucket);

            GetItems.Query query = new GetItems.Query(bucket.Id);
            GetItems.Handler handler = new GetItems.Handler(repository, this.publisher);

            // Act
            GetItems.Response response = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(response.Items);
            Assert.Empty(response.Items);

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    ItemsRetrievedEvent bucketEvent = Assert.IsType<ItemsRetrievedEvent>(publishedEvent.Notification);
                    Assert.Equal("ItemsRetrieved", bucketEvent.Type);
                    Assert.Equal($"Retrieved {response.Items.Count()} Item/s from Bucket {bucket.Id}", bucketEvent.Description);
                    Assert.Equal(bucket.Id, bucketEvent.BucketId);
                    Assert.Empty(bucketEvent.ItemIds);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_GettingItemsFromNonExistingBucket_Expect_BucketNotFoundException(IBucketRepository repository)
        {
            // Arrange
            GetItems.Query query = new GetItems.Query(new Random().Next());
            GetItems.Handler handler = new GetItems.Handler(repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() =>
                handler.Handle(query, default));

            // Assert
            Assert.IsType<BucketNotFoundException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }
    }
}
