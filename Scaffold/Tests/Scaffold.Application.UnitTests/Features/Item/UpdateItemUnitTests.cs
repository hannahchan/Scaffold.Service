namespace Scaffold.Application.UnitTests.Features.Item
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using FluentValidation.TestHelper;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Features.Item;
    using Scaffold.Application.Interfaces;
    using Scaffold.Data;
    using Scaffold.Data.Repositories;
    using Scaffold.Domain.Entities;
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
            public void When_SettingCreatedToTrue_Expect_CreatedTrueAndUpdatedFalse()
            {
                // Arrange
                UpdateItem.Response response = new UpdateItem.Response();

                // Act
                response.Created = true;

                // Assert
                Assert.True(response.Created);
                Assert.False(response.Updated);
            }

            [Fact]
            public void When_SettingCreatedToFalse_Expect_CreatedFalseAndUpdatedTrue()
            {
                // Arrange
                UpdateItem.Response response = new UpdateItem.Response();

                // Act
                response.Created = false;

                // Assert
                Assert.False(response.Created);
                Assert.True(response.Updated);
            }

            [Fact]
            public void When_SettingUpdatedToTrue_Expect_CreatedFalseAndUpdatedTrue()
            {
                // Arrange
                UpdateItem.Response response = new UpdateItem.Response();

                // Act
                response.Updated = true;

                // Assert
                Assert.False(response.Created);
                Assert.True(response.Updated);
            }

            [Fact]
            public void When_SettingUpdatedToFalse_Expect_CreatedTrueAndUpdatedFalse()
            {
                // Arrange
                UpdateItem.Response response = new UpdateItem.Response();

                // Act
                response.Updated = false;

                // Assert
                Assert.True(response.Created);
                Assert.False(response.Updated);
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
                validator.ShouldHaveValidationErrorFor(command => command.Name, value: null);
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
                UpdateItem.Response response = await handler.Handle(command, default(CancellationToken));

                // Assert
                Assert.False(response.Created);
                Assert.True(response.Updated);
                Assert.Equal(bucket.Id, response.Item.Bucket.Id);
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
                UpdateItem.Response response = await handler.Handle(command, default(CancellationToken));

                // Assert
                Assert.True(response.Created);
                Assert.False(response.Updated);
                Assert.Equal(bucket.Id, response.Item.Bucket.Id);
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
                    handler.Handle(command, default(CancellationToken)));

                // Assert
                Assert.NotNull(exception);
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
                    handler.Handle(command, default(CancellationToken)));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ValidationException>(exception);
            }

            [Fact(Skip = "Not Implemented")]
            public void When_UpdatingItemResultingInDomainConflict_Expect_DomainException()
            {
                // Not Implemented
            }

            [Fact(Skip = "Not Implemented")]
            public void When_UpdatingNonExistingItemResultingInDomainConflict_Expect_DomainException()
            {
                // Not Implemented
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

            [Fact]
            public void When_MappingCommandToItemWithEmptyStringProperties_Expect_NullMappedToStringProperties()
            {
                // Arrange
                Item item = new Item { Name = "abc", Description = "xyz" };
                UpdateItem.Command command = new UpdateItem.Command { Name = string.Empty, Description = string.Empty };

                MapperConfiguration configuration = new MapperConfiguration(config =>
                    config.AddProfile(new UpdateItem.MappingProfile()));

                // Act
                item = configuration.CreateMapper().Map<UpdateItem.Command, Item>(command, item);

                // Assert
                Assert.Null(item.Name);
                Assert.Null(item.Description);
            }
        }
    }
}
