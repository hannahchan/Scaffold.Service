namespace Scaffold.Application.UnitTests.Components.Bucket;

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Aggregates.Bucket;
using Scaffold.Domain.Base;
using Xunit;

public class UpdateBucketUnitTests
{
    private readonly Mock.Publisher publisher = new Mock.Publisher();

    public class Handler : UpdateBucketUnitTests
    {
        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_UpdatingBucket_Expect_UpdatedBucked(IBucketRepository repository)
        {
            // Arrange
            Bucket bucket = new Bucket
            {
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Size = new Random().Next(),
            };

            repository.Add(bucket);

            UpdateBucket.Command command = new UpdateBucket.Command(
                Id: bucket.Id,
                Name: Guid.NewGuid().ToString(),
                Description: Guid.NewGuid().ToString(),
                Size: new Random().Next());

            UpdateBucket.Handler handler = new UpdateBucket.Handler(repository, this.publisher);

            // Act
            UpdateBucket.Response response = await handler.Handle(command, default);

            // Assert
            Assert.False(response.Created);
            Assert.Equal(command.Id, response.Bucket.Id);
            Assert.Equal(command.Name, response.Bucket.Name);
            Assert.Equal(command.Description, response.Bucket.Description);
            Assert.Equal(command.Size, response.Bucket.Size);

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketUpdatedEvent bucketEvent = Assert.IsType<BucketUpdatedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketUpdated", bucketEvent.Type);
                    Assert.Equal($"Updated Bucket {response.Bucket.Id}", bucketEvent.Description);
                    Assert.Equal(response.Bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_UpdatingNonExistingBucket_Expect_NewBucket(IBucketRepository repository)
        {
            // Arrange
            UpdateBucket.Command command = new UpdateBucket.Command(
                Id: new Random().Next(),
                Name: Guid.NewGuid().ToString(),
                Description: Guid.NewGuid().ToString(),
                Size: new Random().Next());

            UpdateBucket.Handler handler = new UpdateBucket.Handler(repository, this.publisher);

            // Act
            UpdateBucket.Response response = await handler.Handle(command, default);

            // Assert
            Assert.True(response.Created);
            Assert.Equal(command.Id, response.Bucket.Id);
            Assert.Equal(command.Name, response.Bucket.Name);
            Assert.Equal(command.Description, response.Bucket.Description);
            Assert.Equal(command.Size, response.Bucket.Size);

            Assert.Collection(
                this.publisher.PublishedEvents,
                publishedEvent =>
                {
                    BucketAddedEvent bucketEvent = Assert.IsType<BucketAddedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketAdded", bucketEvent.Type);
                    Assert.Equal($"Added Bucket {response.Bucket.Id}", bucketEvent.Description);
                    Assert.Equal(response.Bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_UpdatingBucketResultingInDomainConflict_Expect_DomainException(IBucketRepository repository)
        {
            // Arrange
            Bucket bucket = new Bucket();
            bucket.AddItem(new Item());
            repository.Add(bucket);

            UpdateBucket.Command command = new UpdateBucket.Command(
                Id: bucket.Id,
                Name: Guid.NewGuid().ToString(),
                Description: null,
                Size: 0);

            UpdateBucket.Handler handler = new UpdateBucket.Handler(repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() =>
                handler.Handle(command, default));

            // Assert
            Assert.IsAssignableFrom<DomainException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_UpdatingNonExistingBucketResultingInDomainConflict_Expect_DomainException(IBucketRepository repository)
        {
            // Arrange
            UpdateBucket.Command command = new UpdateBucket.Command(
                Id: new Random().Next(),
                Name: Guid.NewGuid().ToString(),
                Description: null,
                Size: -1);

            UpdateBucket.Handler handler = new UpdateBucket.Handler(repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() =>
                handler.Handle(command, default));

            // Assert
            Assert.IsAssignableFrom<DomainException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }
    }

    public class MappingProfile
    {
        [Fact]
        public void IsValid()
        {
            // Arrange
            UpdateBucket.MappingProfile profile = new UpdateBucket.MappingProfile();
            MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(profile));

            // Act and Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
