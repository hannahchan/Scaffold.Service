namespace Scaffold.Application.UnitTests.Components.Bucket;

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Aggregates.Bucket;
using Scaffold.Repositories;
using Xunit;

public class UpdateItemUnitTests
{
    private readonly IBucketRepository repository;

    private readonly Mock.Publisher publisher;

    public UpdateItemUnitTests()
    {
        BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        this.repository = new ScopedBucketRepository(context);
        this.publisher = new Mock.Publisher();
    }

    public class Handler : UpdateItemUnitTests
    {
        [Fact]
        public async Task When_UpdatingItemFromBucket_Expect_ItemUpdated()
        {
            // Arrange
            Bucket bucket = new Bucket();
            Item item = new Item { Name = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString() };
            bucket.AddItem(item);

            await this.repository.AddAsync(bucket);

            UpdateItem.Command command = new UpdateItem.Command(
                BucketId: bucket.Id,
                ItemId: item.Id,
                Name: Guid.NewGuid().ToString(),
                Description: Guid.NewGuid().ToString());

            UpdateItem.Handler handler = new UpdateItem.Handler(this.repository, this.publisher);

            // Act
            UpdateItem.Response response = await handler.Handle(command, default);

            // Assert
            Assert.False(response.Created);
            Assert.Equal(command.ItemId, response.Item.Id);
            Assert.Equal(command.Name, response.Item.Name);
            Assert.Equal(command.Description, response.Item.Description);

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    ItemUpdatedEvent bucketEvent = Assert.IsType<ItemUpdatedEvent>(publishedEvent.Notification);
                    Assert.Equal(typeof(UpdateItem.Handler), bucketEvent.Source);
                    Assert.Equal("ItemUpdated", bucketEvent.Type);
                    Assert.Equal($"Updated Item {item.Id} in Bucket {bucket.Id}", bucketEvent.Description);
                    Assert.Equal(bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(item.Id, bucketEvent.ItemId);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Fact]

        public async Task When_UpdatingNonExistingItemFromBucket_Expect_NewItem()
        {
            // Arrange
            Bucket bucket = new Bucket();
            await this.repository.AddAsync(bucket);

            UpdateItem.Command command = new UpdateItem.Command(
                BucketId: bucket.Id,
                ItemId: new Random().Next(),
                Name: Guid.NewGuid().ToString(),
                Description: Guid.NewGuid().ToString());

            UpdateItem.Handler handler = new UpdateItem.Handler(this.repository, this.publisher);

            // Act
            UpdateItem.Response response = await handler.Handle(command, default);

            // Assert
            Assert.True(response.Created);
            Assert.Equal(command.ItemId, response.Item.Id);
            Assert.Equal(command.Name, response.Item.Name);
            Assert.Equal(command.Description, response.Item.Description);

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    ItemAddedEvent bucketEvent = Assert.IsType<ItemAddedEvent>(publishedEvent.Notification);
                    Assert.Equal(typeof(UpdateItem.Handler), bucketEvent.Source);
                    Assert.Equal("ItemAdded", bucketEvent.Type);
                    Assert.Equal($"Added Item {response.Item.Id} to Bucket {bucket.Id}", bucketEvent.Description);
                    Assert.Equal(bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(response.Item.Id, bucketEvent.ItemId);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Fact]
        public async Task When_UpdatingItemFromNonExistingBucket_Expect_BucketNotFoundException()
        {
            // Arrange
            UpdateItem.Command command = new UpdateItem.Command(
                BucketId: new Random().Next(),
                ItemId: new Random().Next(),
                Name: Guid.NewGuid().ToString(),
                Description: null);

            UpdateItem.Handler handler = new UpdateItem.Handler(this.repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() =>
                handler.Handle(command, default));

            // Assert
            Assert.IsType<BucketNotFoundException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }
    }

    public class MappingProfile
    {
        [Fact]
        public void IsValid()
        {
            // Arrange
            UpdateItem.MappingProfile profile = new UpdateItem.MappingProfile();
            MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(profile));

            // Act and Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
