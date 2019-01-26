namespace Scaffold.Application.UnitTests.Features.Item
{
    using System;
    using AutoMapper;
    using FluentValidation.TestHelper;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Features.Item;
    using Scaffold.Application.Repositories;
    using Scaffold.Data;
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
            public void When_UpdatingItemFromBucket_Expect_ItemUpdated()
            {
            }

            [Fact]

            public void When_UpdatingNonExistingItemFromBucket_Expect_ItemNotFoundException()
            {
            }

            [Fact]

            public void When_UpdatingItemFromNonExistingBucket_Expect_BucketNotFoundException()
            {
            }

            [Fact]

            public void When_UpdatingItemWithInvalidCommand_Expect_ValidationException()
            {
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
