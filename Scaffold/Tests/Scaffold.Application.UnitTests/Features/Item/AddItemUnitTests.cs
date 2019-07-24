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
    using Scaffold.Domain.Exceptions;
    using Scaffold.Repositories.EntityFrameworkCore;
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

        public class Validator
        {
            [Fact]
            public void ShouldNotHaveValidationErrorFor()
            {
                // Arrange
                AddItem.Validator validator = new AddItem.Validator();

                // Act and Assert
                validator.ShouldNotHaveValidationErrorFor(command => command.Name, Guid.NewGuid().ToString());
                validator.ShouldNotHaveValidationErrorFor(command => command.Description, Guid.NewGuid().ToString());
            }

            [Fact]
            public void ShouldHaveValidationErrorFor()
            {
                // Arrange
                AddItem.Validator validator = new AddItem.Validator();

                // Act and Assert
                validator.ShouldHaveValidationErrorFor(command => command.Name, string.Empty);
                validator.ShouldHaveValidationErrorFor(command => command.Name, null as string);
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

                AddItem.Command command = new AddItem.Command
                {
                    BucketId = bucket.Id,
                    Name = Guid.NewGuid().ToString(),
                };

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
                AddItem.Command command = new AddItem.Command
                {
                    BucketId = new Random().Next(int.MaxValue),
                    Name = Guid.NewGuid().ToString(),
                };

                AddItem.Handler handler = new AddItem.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<BucketNotFoundException>(exception);
            }

            [Fact]
            public async Task When_AddingItemWithInvalidCommand_Expect_ValidationException()
            {
                // Arrange
                AddItem.Command command = new AddItem.Command { Name = string.Empty };
                AddItem.Handler handler = new AddItem.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ValidationException>(exception);
            }

            [Fact]
            public async Task When_AddingItemToFullBucket_Expect_BucketFullException()
            {
                // Arrange
                Bucket bucket = new Bucket { Size = 0 };
                await this.repository.AddAsync(bucket);

                AddItem.Command command = new AddItem.Command
                {
                    BucketId = bucket.Id,
                    Name = Guid.NewGuid().ToString(),
                };

                AddItem.Handler handler = new AddItem.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default));

                // Assert
                Assert.NotNull(exception);
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
