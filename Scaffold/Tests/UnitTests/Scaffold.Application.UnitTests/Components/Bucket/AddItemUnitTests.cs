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

public class AddItemUnitTests
{
    private readonly IBucketRepository repository;

    private readonly Mock.Publisher publisher;

    public AddItemUnitTests()
    {
        BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        this.repository = new ScopedBucketRepository(context);
        this.publisher = new Mock.Publisher();
    }

    public class Handler : AddItemUnitTests
    {
        [Fact]
        public async Task When_AddingItemToBucket_Expect_AddedItem()
        {
            // Arrange
            Bucket bucket = new Bucket();
            await this.repository.AddAsync(bucket);

            AddItem.Command command = new AddItem.Command(
                BucketId: bucket.Id,
                Name: Guid.NewGuid().ToString(),
                Description: null);

            AddItem.Handler handler = new AddItem.Handler(this.repository, this.publisher);

            // Act
            AddItem.Response response = await handler.Handle(command, default);

            // Assert
            Assert.NotEqual(default, response.Item.Id);
            Assert.Equal(command.Name, response.Item.Name);

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    ItemAddedEvent bucketEvent = Assert.IsType<ItemAddedEvent>(publishedEvent.Notification);
                    Assert.Equal("ItemAdded", bucketEvent.Type);
                    Assert.Equal($"Added Item {response.Item.Id} to Bucket {bucket.Id}", bucketEvent.Description);
                    Assert.Equal(bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(response.Item.Id, bucketEvent.ItemId);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Fact]
        public async Task When_AddingItemToNonExistingBucket_Expect_BucketNotFoundException()
        {
            // Arrange
            AddItem.Command command = new AddItem.Command(
                BucketId: new Random().Next(),
                Name: Guid.NewGuid().ToString(),
                Description: null);

            AddItem.Handler handler = new AddItem.Handler(this.repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() =>
                handler.Handle(command, default));

            // Assert
            Assert.IsType<BucketNotFoundException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }

        [Fact]
        public async Task When_AddingItemToFullBucket_Expect_BucketFullException()
        {
            // Arrange
            Bucket bucket = new Bucket { Size = 0 };
            await this.repository.AddAsync(bucket);

            AddItem.Command command = new AddItem.Command(
                BucketId: bucket.Id,
                Name: Guid.NewGuid().ToString(),
                Description: null);

            AddItem.Handler handler = new AddItem.Handler(this.repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() =>
                handler.Handle(command, default));

            // Assert
            Assert.IsType<BucketFullException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }
    }

    public class MappingProfile
    {
        [Fact]
        public void IsValid()
        {
            // Arrange
            AddItem.MappingProfile profile = new AddItem.MappingProfile();
            MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(profile));

            // Act and Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
