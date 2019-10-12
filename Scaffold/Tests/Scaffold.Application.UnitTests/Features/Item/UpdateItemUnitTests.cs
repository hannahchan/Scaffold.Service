namespace Scaffold.Application.UnitTests.Features.Item
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using FluentValidation.TestHelper;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Features.Item;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Repositories.EntityFrameworkCore;
    using Xunit;

    public class UpdateItemUnitTests
    {
        private readonly IBucketRepository repository;

        public UpdateItemUnitTests()
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
                UpdateItem.Response response = new UpdateItem.Response(item);

                // Assert
                Assert.Equal(item, response.Item);
                Assert.False(response.Created);
                Assert.True(response.Updated);
            }

            [Fact]
            public void When_InstantiatingResponseWithCreatedSetToTrue_Expect_CreatedTrue()
            {
                // Arrange
                Item item = new Item();

                // Act
                UpdateItem.Response response = new UpdateItem.Response(item, true);

                // Assert
                Assert.Equal(item, response.Item);
                Assert.True(response.Created);
                Assert.False(response.Updated);
            }

            [Fact]
            public void When_InstantiatingResponseWithCreatedSetToFalse_Expect_CreatedFalse()
            {
                // Arrange
                Item item = new Item();

                // Act
                UpdateItem.Response response = new UpdateItem.Response(item, false);

                // Assert
                Assert.Equal(item, response.Item);
                Assert.False(response.Created);
                Assert.True(response.Updated);
            }

            [Fact]
            public void When_InstantiatingResponseWithNull_Expect_ArgumentNullException()
            {
                // Act
                Exception exception = Record.Exception(() => new UpdateItem.Response(null!));

                // Assert
                Assert.IsType<ArgumentNullException>(exception);
            }
        }

        public class Validator
        {
            [Fact]
            public void ShouldNotHaveValidationErrorFor()
            {
                // Arrange
                UpdateItem.Validator validator = new UpdateItem.Validator();

                // Act and Assert
                validator.ShouldNotHaveValidationErrorFor(command => command.BucketId, new Random().Next(int.MaxValue));
                validator.ShouldNotHaveValidationErrorFor(command => command.ItemId, new Random().Next(int.MaxValue));
                validator.ShouldNotHaveValidationErrorFor(command => command.Name, Guid.NewGuid().ToString());
            }

            [Fact]
            public void ShouldHaveValidationErrorFor()
            {
                // Arrange
                UpdateItem.Validator validator = new UpdateItem.Validator();

                // Act and Assert
                validator.ShouldHaveValidationErrorFor(command => command.BucketId, default(int));
                validator.ShouldHaveValidationErrorFor(command => command.ItemId, default(int));
                validator.ShouldHaveValidationErrorFor(command => command.Name, string.Empty);
                validator.ShouldHaveValidationErrorFor(command => command.Name, null as string);
            }
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

                UpdateItem.Command command = new UpdateItem.Command
                {
                    BucketId = bucket.Id,
                    ItemId = item.Id,
                    Name = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                };

                UpdateItem.Handler handler = new UpdateItem.Handler(this.repository);

                // Act
                UpdateItem.Response response = await handler.Handle(command, default);

                // Assert
                Assert.False(response.Created);
                Assert.True(response.Updated);
                Assert.Equal(item.Id, response.Item.Id);
                Assert.Equal(command.Name, response.Item.Name);
                Assert.Equal(command.Description, response.Item.Description);
            }

            [Fact]

            public async Task When_UpdatingNonExistingItemFromBucket_Expect_NewItem()
            {
                // Arrange
                Bucket bucket = new Bucket();
                await this.repository.AddAsync(bucket);

                UpdateItem.Command command = new UpdateItem.Command
                {
                    BucketId = bucket.Id,
                    ItemId = new Random().Next(int.MaxValue),
                    Name = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                };

                UpdateItem.Handler handler = new UpdateItem.Handler(this.repository);

                // Act
                UpdateItem.Response response = await handler.Handle(command, default);

                // Assert
                Assert.True(response.Created);
                Assert.False(response.Updated);
                Assert.Equal(command.ItemId, response.Item.Id);
                Assert.Equal(command.Name, response.Item.Name);
                Assert.Equal(command.Description, response.Item.Description);
            }

            [Fact]
            public async Task When_UpdatingItemFromNonExistingBucket_Expect_BucketNotFoundException()
            {
                // Arrange
                UpdateItem.Command command = new UpdateItem.Command
                {
                    BucketId = new Random().Next(int.MaxValue),
                    ItemId = new Random().Next(int.MaxValue),
                    Name = Guid.NewGuid().ToString(),
                };

                UpdateItem.Handler handler = new UpdateItem.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default));

                // Assert
                Assert.IsType<BucketNotFoundException>(exception);
            }

            [Fact]
            public async Task When_UpdatingItemWithInvalidCommand_Expect_ValidationException()
            {
                // Arrange
                UpdateItem.Command command = new UpdateItem.Command { Name = string.Empty };
                UpdateItem.Handler handler = new UpdateItem.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default));

                // Assert
                Assert.IsType<ValidationException>(exception);
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
}
