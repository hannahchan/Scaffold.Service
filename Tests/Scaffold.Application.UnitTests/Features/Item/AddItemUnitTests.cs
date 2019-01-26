namespace Scaffold.Application.UnitTests.Features.Item
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using FluentValidation.TestHelper;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Features.Item;
    using Scaffold.Application.Repositories;
    using Scaffold.Data;
    using Scaffold.Domain.Entities;
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
                validator.ShouldHaveValidationErrorFor(command => command.Name, value: null);
            }
        }

        public class Handler : AddItemUnitTests
        {
            [Fact]
            public async Task When_AddingNewItemToBucket_Expect_AddedItem()
            {
                // Arrange
                Bucket bucket = new Bucket();
                await this.repository.AddAsync(bucket);

                AddItem.Command command = new AddItem.Command
                {
                    BucketId = bucket.Id,
                    Name = Guid.NewGuid().ToString()
                };

                AddItem.Handler handler = new AddItem.Handler(this.repository);

                // Act
                AddItem.Response response = await handler.Handle(command, default(CancellationToken));

                // Assert
                Assert.NotEqual(default(int), response.Item.Id);
                Assert.Equal(command.Name, response.Item.Name);
                Assert.Equal(bucket, response.Item.Bucket);
                Assert.Contains(response.Item, bucket.Items);
            }

            [Fact]
            public async Task When_AddingNewItemToNonExistingBucket_Expect_BucketNotFoundException()
            {
                // Arrange
                AddItem.Command command = new AddItem.Command
                {
                    BucketId = new Random().Next(int.MaxValue),
                    Name = Guid.NewGuid().ToString()
                };

                AddItem.Handler handler = new AddItem.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default(CancellationToken)));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<BucketNotFoundException>(exception);
            }

            [Fact]
            public async Task When_AddingNewItemWithInvalidCommand_Expect_ValidationException()
            {
                // Arrange
                AddItem.Command command = new AddItem.Command { Name = string.Empty };
                AddItem.Handler handler = new AddItem.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default(CancellationToken)));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ValidationException>(exception);
            }
        }
    }
}
