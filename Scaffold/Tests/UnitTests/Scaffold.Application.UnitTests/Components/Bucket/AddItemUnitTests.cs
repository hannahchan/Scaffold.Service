namespace Scaffold.Application.UnitTests.Components.Bucket;

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Aggregates.Bucket;
using Xunit;

public class AddItemUnitTests
{
    private readonly Mock.Publisher publisher = new Mock.Publisher();

    public class Handler : AddItemUnitTests
    {
        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_AddingItemToBucket_Expect_AddedItem(IBucketRepository repository)
        {
            // Arrange
            Bucket bucket = new Bucket();
            repository.Add(bucket);

            AddItem.Command command = new AddItem.Command(
                BucketId: bucket.Id,
                Name: Guid.NewGuid().ToString(),
                Description: null);

            AddItem.Handler handler = new AddItem.Handler(repository, this.publisher);

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

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_AddingItemToNonExistingBucket_Expect_BucketNotFoundException(IBucketRepository repository)
        {
            // Arrange
            AddItem.Command command = new AddItem.Command(
                BucketId: new Random().Next(),
                Name: Guid.NewGuid().ToString(),
                Description: null);

            AddItem.Handler handler = new AddItem.Handler(repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() =>
                handler.Handle(command, default));

            // Assert
            Assert.IsType<BucketNotFoundException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_AddingItemToFullBucket_Expect_BucketFullException(IBucketRepository repository)
        {
            // Arrange
            Bucket bucket = new Bucket { Size = 0 };
            repository.Add(bucket);

            AddItem.Command command = new AddItem.Command(
                BucketId: bucket.Id,
                Name: Guid.NewGuid().ToString(),
                Description: null);

            AddItem.Handler handler = new AddItem.Handler(repository, this.publisher);

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
