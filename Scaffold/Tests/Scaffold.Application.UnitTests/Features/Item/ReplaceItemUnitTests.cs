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

    public class ReplaceItemUnitTests
    {
        private readonly IBucketRepository repository;

        public ReplaceItemUnitTests()
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
                ReplaceItem.Validator validator = new ReplaceItem.Validator();

                // Act and Assert
                validator.ShouldNotHaveValidationErrorFor(command => command.BucketId, new Random().Next(int.MaxValue));
                validator.ShouldNotHaveValidationErrorFor(command => command.ItemId, new Random().Next(int.MaxValue));
                validator.ShouldNotHaveValidationErrorFor(command => command.Name, Guid.NewGuid().ToString());
            }

            [Fact]
            public void ShouldHaveValidationErrorFor()
            {
                // Arrange
                ReplaceItem.Validator validator = new ReplaceItem.Validator();

                // Act and Assert
                validator.ShouldHaveValidationErrorFor(command => command.BucketId, default(int));
                validator.ShouldHaveValidationErrorFor(command => command.ItemId, default(int));
                validator.ShouldHaveValidationErrorFor(command => command.Name, string.Empty);
                validator.ShouldHaveValidationErrorFor(command => command.Name, value: null);
            }
        }

        public class Handler : ReplaceItemUnitTests
        {
            [Fact]
            public async Task When_UpdatingItemFromBucket_Expect_ItemReplaced()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item { Name = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString() };
                bucket.AddItem(item);

                await this.repository.AddAsync(bucket);

                ReplaceItem.Command command = new ReplaceItem.Command
                {
                    BucketId = bucket.Id,
                    ItemId = item.Id,
                    Name = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                };

                ReplaceItem.Handler handler = new ReplaceItem.Handler(this.repository);

                // Act
                ReplaceItem.Response response = await handler.Handle(command, default(CancellationToken));

                // Assert
                Assert.False(response.Created);
                Assert.True(response.Replaced);
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

                ReplaceItem.Command command = new ReplaceItem.Command
                {
                    BucketId = bucket.Id,
                    ItemId = new Random().Next(int.MaxValue),
                    Name = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                };

                ReplaceItem.Handler handler = new ReplaceItem.Handler(this.repository);

                // Act
                ReplaceItem.Response response = await handler.Handle(command, default(CancellationToken));

                // Assert
                Assert.True(response.Created);
                Assert.False(response.Replaced);
                Assert.Equal(bucket.Id, response.Item.Bucket.Id);
                Assert.Equal(command.ItemId, response.Item.Id);
                Assert.Equal(command.Name, response.Item.Name);
                Assert.Equal(command.Description, response.Item.Description);
            }

            [Fact]
            public async Task When_UpdatingItemFromNonExistingBucket_Expect_BucketNotFoundException()
            {
                // Arrange
                ReplaceItem.Command command = new ReplaceItem.Command
                {
                    BucketId = new Random().Next(int.MaxValue),
                    ItemId = new Random().Next(int.MaxValue),
                    Name = Guid.NewGuid().ToString(),
                };

                ReplaceItem.Handler handler = new ReplaceItem.Handler(this.repository);

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
                ReplaceItem.Command command = new ReplaceItem.Command { Name = string.Empty };
                ReplaceItem.Handler handler = new ReplaceItem.Handler(this.repository);

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
                ReplaceItem.MappingProfile profile = new ReplaceItem.MappingProfile();
                MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(profile));

                // Act and Assert
                configuration.AssertConfigurationIsValid();
            }

            [Fact]
            public void When_MappingCommandToItemWithEmptyStringProperties_Expect_NullMappedToStringProperties()
            {
                // Arrange
                Item item = new Item { Name = "abc", Description = "xyz" };
                ReplaceItem.Command command = new ReplaceItem.Command { Name = string.Empty, Description = string.Empty };

                MapperConfiguration configuration = new MapperConfiguration(config =>
                    config.AddProfile(new ReplaceItem.MappingProfile()));

                // Act
                item = configuration.CreateMapper().Map<ReplaceItem.Command, Item>(command, item);

                // Assert
                Assert.Null(item.Name);
                Assert.Null(item.Description);
            }
        }
    }
}
