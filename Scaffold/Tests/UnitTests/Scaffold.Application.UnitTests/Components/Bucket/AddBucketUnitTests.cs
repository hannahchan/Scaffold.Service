namespace Scaffold.Application.UnitTests.Components.Bucket;

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Base;
using Xunit;

public class AddBucketUnitTests
{
    private readonly Mock.Publisher publisher = new Mock.Publisher();

    public class Handler : AddBucketUnitTests
    {
        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_AddingBucket_Expect_AddedBucket(IBucketRepository repository)
        {
            // Arrange
            AddBucket.Command command = new AddBucket.Command(
                Name: Guid.NewGuid().ToString(),
                Description: null,
                Size: null);

            AddBucket.Handler handler = new AddBucket.Handler(repository, this.publisher);

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
                    BucketAddedEvent bucketEvent = Assert.IsType<BucketAddedEvent>(publishedEvent.Notification);
                    Assert.Equal("BucketAdded", bucketEvent.Type);
                    Assert.Equal($"Added Bucket {response.Bucket.Id}", bucketEvent.Description);
                    Assert.Equal(response.Bucket.Id, bucketEvent.BucketId);
                    Assert.Equal(CancellationToken.None, publishedEvent.CancellationToken);
                });
        }

        [Theory]
        [ClassData(typeof(TestRepositories))]
        public async Task When_AddingBucketResultingInDomainConflict_Expect_DomainException(IBucketRepository repository)
        {
            // Arrange
            AddBucket.Command command = new AddBucket.Command(
                Name: Guid.NewGuid().ToString(),
                Description: Guid.NewGuid().ToString(),
                Size: -1);

            AddBucket.Handler handler = new AddBucket.Handler(repository, this.publisher);

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
