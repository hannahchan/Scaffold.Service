namespace Scaffold.Application.UnitTests.Features.Bucket
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Repositories;
    using Xunit;

    public class AddItemUnitTests
    {
        private readonly IBucketRepository repository;

        public AddItemUnitTests()
        {
            BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            this.repository = new BucketRepository(context);
        }

        public class Response
        {
            [Fact]
            public void When_InstantiatingResponseWithItem_Expect_ResponseWithItem()
            {
                // Arrange
                Item item = new Item();

                // Act
                AddItem.Response response = new AddItem.Response(item);

                // Assert
                Assert.Equal(item, response.Item);
            }

            [Fact]
            public void When_InstantiatingResponseWithNull_Expect_ArgumentNullException()
            {
                // Act
                Exception exception = Record.Exception(() => new AddItem.Response(null));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("item", argumentNullException.ParamName);
            }
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
                    bucketId: bucket.Id,
                    name: Guid.NewGuid().ToString(),
                    description: null);

                AddItem.Handler handler = new AddItem.Handler(this.repository);

                // Act
                AddItem.Response response = await handler.Handle(command, default);

                // Assert
                Assert.NotEqual(default, response.Item.Id);
                Assert.Equal(command.Name, response.Item.Name);
            }

            [Fact]
            public async Task When_AddingItemToNonExistingBucket_Expect_BucketNotFoundException()
            {
                // Arrange
                AddItem.Command command = new AddItem.Command(
                    bucketId: new Random().Next(),
                    name: Guid.NewGuid().ToString(),
                    description: null);

                AddItem.Handler handler = new AddItem.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default));

                // Assert
                Assert.IsType<BucketNotFoundException>(exception);
            }

            [Fact]
            public async Task When_AddingItemToFullBucket_Expect_BucketFullException()
            {
                // Arrange
                Bucket bucket = new Bucket { Size = 0 };
                await this.repository.AddAsync(bucket);

                AddItem.Command command = new AddItem.Command(
                    bucketId: bucket.Id,
                    name: Guid.NewGuid().ToString(),
                    description: null);

                AddItem.Handler handler = new AddItem.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default));

                // Assert
                Assert.IsType<BucketFullException>(exception);
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
}
