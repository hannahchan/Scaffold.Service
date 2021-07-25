namespace Scaffold.Application.UnitTests.Components.Bucket
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Components.Bucket;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Domain.Base;
    using Scaffold.Repositories;
    using Xunit;

    public class UpdateBucketUnitTests
    {
        private readonly IBucketRepository repository;

        public UpdateBucketUnitTests()
        {
            BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            this.repository = new BucketRepository(context);
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

                UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository);

                // Act
                UpdateBucket.Response response = await handler.Handle(command, default);

                // Assert
                Assert.False(response.Created);
                Assert.Equal(bucket.Id, response.Bucket.Id);
                Assert.Equal(bucket.Name, response.Bucket.Name);
                Assert.Equal(bucket.Description, response.Bucket.Description);
                Assert.Equal(bucket.Size, response.Bucket.Size);
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

                UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository);

                // Act
                UpdateBucket.Response response = await handler.Handle(command, default);

                // Assert
                Assert.True(response.Created);
                Assert.Equal(command.Id, response.Bucket.Id);
                Assert.Equal(command.Name, response.Bucket.Name);
                Assert.Equal(command.Description, response.Bucket.Description);
                Assert.Equal(command.Size, response.Bucket.Size);
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

                UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default));

                // Assert
                Assert.IsAssignableFrom<DomainException>(exception);
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

                UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default));

                // Assert
                Assert.IsAssignableFrom<DomainException>(exception);
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
}
