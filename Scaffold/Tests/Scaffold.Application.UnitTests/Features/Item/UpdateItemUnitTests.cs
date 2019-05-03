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
                validator.ShouldNotHaveValidationErrorFor(command => command.Name, value: null);
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
            }
        }

        public class Handler : UpdateItemUnitTests
        {
            [Fact]
            public async Task When_UpdatingItemFromBucket_Expect_ItemUpdated()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item { Name = "abc", Description = "def" };
                bucket.AddItem(item);

                await this.repository.AddAsync(bucket);

                UpdateItem.Command command = new UpdateItem.Command
                {
                    BucketId = bucket.Id,
                    ItemId = item.Id,
                    Name = "uvw",
                    Description = "xyz",
                };

                UpdateItem.Handler handler = new UpdateItem.Handler(this.repository);

                // Act
                UpdateItem.Response response = await handler.Handle(command, default(CancellationToken));

                // Assert
                Assert.Equal("uvw", response.Item.Name);
                Assert.Equal("xyz", response.Item.Description);
            }

            [Fact]

            public async Task When_UpdatingNonExistingItemFromBucket_Expect_ItemNotFoundException()
            {
                // Arrange
                Bucket bucket = new Bucket();
                await this.repository.AddAsync(bucket);

                UpdateItem.Command command = new UpdateItem.Command
                {
                    BucketId = bucket.Id,
                    ItemId = new Random().Next(int.MaxValue),
                    Name = Guid.NewGuid().ToString(),
                };

                UpdateItem.Handler handler = new UpdateItem.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default(CancellationToken)));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ItemNotFoundException>(exception);
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
            public void When_MappingCommandToItemWithOnlyNameNotNull_Expect_NameMapped()
            {
                // Arrange
                Item item = new Item { Name = "abc", Description = "xyz" };
                UpdateItem.Command command = new UpdateItem.Command { Name = "def" };

                MapperConfiguration configuration = new MapperConfiguration(config =>
                    config.AddProfile(new UpdateItem.MappingProfile()));

                // Act
                item = configuration.CreateMapper().Map<UpdateItem.Command, Item>(command, item);

                // Assert
                Assert.Equal("def", item.Name);
                Assert.Equal("xyz", item.Description);
            }

            [Fact]
            public void When_MappingCommandToItemWithOnlyDescriptionNotNull_Expect_DescriptionMapped()
            {
                // Arrange
                Item item = new Item { Name = "abc", Description = "xyz" };
                UpdateItem.Command command = new UpdateItem.Command { Description = "uvw" };

                MapperConfiguration configuration = new MapperConfiguration(config =>
                    config.AddProfile(new UpdateItem.MappingProfile()));

                // Act
                item = configuration.CreateMapper().Map<UpdateItem.Command, Item>(command, item);

                // Assert
                Assert.Equal("abc", item.Name);
                Assert.Equal("uvw", item.Description);
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
