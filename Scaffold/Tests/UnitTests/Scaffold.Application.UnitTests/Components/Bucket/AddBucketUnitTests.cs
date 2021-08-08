namespace Scaffold.Application.UnitTests.Components.Bucket
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Common.Messaging;
    using Scaffold.Application.Components.Bucket;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Base;
    using Scaffold.Repositories;
    using Xunit;

    public class AddBucketUnitTests
    {
        private readonly IBucketRepository repository;

        private readonly Mock.Publisher publisher;

        public AddBucketUnitTests()
        {
            BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            this.repository = new BucketRepository(context);
            this.publisher = new Mock.Publisher();
        }

        public class Handler : AddBucketUnitTests
        {
            [Fact]
            public async Task When_AddingBucket_Expect_AddedBucket()
            {
                // Arrange
                AddBucket.Command command = new AddBucket.Command(
                    Name: Guid.NewGuid().ToString(),
                    Description: null,
                    Size: null);

                AddBucket.Handler handler = new AddBucket.Handler(this.repository, this.publisher);

                // Act
                AddBucket.Response response = await handler.Handle(command, default);

                // Assert
                Assert.NotEqual(default, response.Bucket.Id);
                Assert.Equal(command.Name, response.Bucket.Name);
                Assert.NotNull(response.Bucket.Items);

                Assert.Collection(
                    this.publisher.PublishedEvents,
                    publishedEvent =>
                    {
                        BucketAddedEvent<AddBucket.Handler> bucketEvent = Assert.IsType<BucketAddedEvent<AddBucket.Handler>>(publishedEvent.Notification);
                        Assert.Equal("BucketAdded", bucketEvent.Type);
                        Assert.Equal($"Added Bucket {response.Bucket.Id}", bucketEvent.Description);
                        Assert.Equal(response.Bucket.Id, bucketEvent.BucketId);
                        Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                    });
            }

            [Fact]
            public async Task When_AddingBucketResultingInDomainConflict_Expect_DomainException()
            {
                // Arrange
                AddBucket.Command command = new AddBucket.Command(
                    Name: Guid.NewGuid().ToString(),
                    Description: Guid.NewGuid().ToString(),
                    Size: -1);

                AddBucket.Handler handler = new AddBucket.Handler(this.repository, this.publisher);

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
                AddBucket.MappingProfile profile = new AddBucket.MappingProfile();
                MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(profile));

                // Act and Assert
                configuration.AssertConfigurationIsValid();
            }
        }
    }
}
