namespace Scaffold.Application.UnitTests.Components.Bucket;

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Aggregates.Bucket;
using Scaffold.Domain.Base;
using Scaffold.Repositories;
using Xunit;

public class UpdateBucketUnitTests
{
    private readonly IBucketRepository repository;

    private readonly Mock.Publisher publisher;

    public UpdateBucketUnitTests()
    {
        BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        this.repository = new ScopedBucketRepository(context);
        this.publisher = new Mock.Publisher();
    }

    public class Handler : UpdateBucketUnitTests
    {
        [Fact]
        public async Task When_UpdatingBucket_Expect_UpdatedBucked()
        {
            // Arrange
            Bucket bucket = new Bucket
            {
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Size = new Random().Next(),
            };

            await this.repository.AddAsync(bucket);

            UpdateBucket.Command command = new UpdateBucket.Command(
                Id: bucket.Id,
                Name: Guid.NewGuid().ToString(),
                Description: Guid.NewGuid().ToString(),
                Size: new Random().Next());

            UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository, this.publisher);

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
                    BucketUpdatedEvent<UpdateBucket.Handler> bucketEvent = Assert.IsType<BucketUpdatedEvent<UpdateBucket.Handler>>(publishedEvent.Notification);
                    Assert.Equal("BucketUpdated", bucketEvent.Type);
                    Assert.Equal($"Updated Bucket {response.Bucket.Id}", bucketEvent.Description);
                    Assert.Equal(response.Bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Fact]
        public async Task When_UpdatingNonExistingBucket_Expect_NewBucket()
        {
            // Arrange
            UpdateBucket.Command command = new UpdateBucket.Command(
                Id: new Random().Next(),
                Name: Guid.NewGuid().ToString(),
                Description: Guid.NewGuid().ToString(),
                Size: new Random().Next());

            UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository, this.publisher);

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
                    BucketAddedEvent<UpdateBucket.Handler> bucketEvent = Assert.IsType<BucketAddedEvent<UpdateBucket.Handler>>(publishedEvent.Notification);
                    Assert.Equal("BucketAdded", bucketEvent.Type);
                    Assert.Equal($"Added Bucket {response.Bucket.Id}", bucketEvent.Description);
                    Assert.Equal(response.Bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Fact]
        public async Task When_UpdatingBucketResultingInDomainConflict_Expect_DomainException()
        {
            // Arrange
            Bucket bucket = new Bucket();
            bucket.AddItem(new Item());
            await this.repository.AddAsync(bucket);

            UpdateBucket.Command command = new UpdateBucket.Command(
                Id: bucket.Id,
                Name: Guid.NewGuid().ToString(),
                Description: null,
                Size: 0);

            UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository, this.publisher);

            // Act
            Exception exception = await Record.ExceptionAsync(() =>
                handler.Handle(command, default));

            // Assert
            Assert.IsAssignableFrom<DomainException>(exception);
            Assert.Empty(this.publisher.PublishedEvents);
        }

        [Fact]
        public async Task When_UpdatingNonExistingBucketResultingInDomainConflict_Expect_DomainException()
        {
            // Arrange
            UpdateBucket.Command command = new UpdateBucket.Command(
                Id: new Random().Next(),
                Name: Guid.NewGuid().ToString(),
                Description: null,
                Size: -1);

            UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository, this.publisher);

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
