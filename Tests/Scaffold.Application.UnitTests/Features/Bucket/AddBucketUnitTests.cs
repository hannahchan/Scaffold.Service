namespace Scaffold.Application.UnitTests.Features.Bucket
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using FluentValidation.TestHelper;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.Application.Repositories;
    using Scaffold.Data;
    using Xunit;

    public class AddBucketUnitTests
    {
        private readonly IBucketRepository repository;

        public AddBucketUnitTests()
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
                AddBucket.Validator validator = new AddBucket.Validator();

                // Act and Assert
                validator.ShouldNotHaveValidationErrorFor(command => command.Name, Guid.NewGuid().ToString());
                validator.ShouldNotHaveValidationErrorFor(command => command.Description, Guid.NewGuid().ToString());
            }

            [Fact]
            public void ShouldHaveValidationErrorFor()
            {
                // Arrange
                AddBucket.Validator validator = new AddBucket.Validator();

                // Act and Assert
                validator.ShouldHaveValidationErrorFor(command => command.Name, string.Empty);
                validator.ShouldHaveValidationErrorFor(command => command.Name, null as string);
            }
        }

        public class Handler : AddBucketUnitTests
        {
            [Fact]
            public async Task When_AddingNewBucket_Expect_AddedBucket()
            {
                // Arrange
                AddBucket.Command command = new AddBucket.Command
                {
                    Name = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString()
                };

                AddBucket.Handler handler = new AddBucket.Handler(this.repository);

                // Act
                AddBucket.Response response = await handler.Handle(command, default(CancellationToken));

                // Assert
                Assert.NotEqual(default(int), response.Bucket.Id);
                Assert.Equal(command.Name, response.Bucket.Name);
                Assert.NotNull(response.Bucket.Items);
            }

            [Fact]
            public async Task When_AddingNewBucketWithInvalidCommand_Expect_ValidationException()
            {
                // Arrange
                AddBucket.Command command = new AddBucket.Command { Name = string.Empty };
                AddBucket.Handler handler = new AddBucket.Handler(this.repository);

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
