namespace Scaffold.Application.UnitTests.Features.Bucket
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using FluentValidation.TestHelper;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.Application.Repositories;
    using Scaffold.Data;
    using Scaffold.Domain.Entities;
    using Scaffold.Domain.Exceptions;
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
                validator.ShouldHaveValidationErrorFor(command => command.Name, value: null);
            }
        }

        public class Handler : AddBucketUnitTests
        {
            [Fact]
            public async Task When_AddingBucket_Expect_AddedBucket()
            {
                // Arrange
                AddBucket.Command command = new AddBucket.Command
                {
                    Name = Guid.NewGuid().ToString(),
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
            public async Task When_AddingBucketWithInvalidCommand_Expect_ValidationException()
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

            [Fact]
            public async Task When_AddingBucketResultingInDomainConflict_Expect_DomainException()
            {
                // Arrange
                AddBucket.Command command = new AddBucket.Command
                {
                    Name = Guid.NewGuid().ToString(),
                    Description= Guid.NewGuid().ToString(),
                    Size = -1
                };
                AddBucket.Handler handler = new AddBucket.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default(CancellationToken)));

                // Assert
                Assert.NotNull(exception);
                Assert.IsAssignableFrom<DomainException>(exception);
            }
        }

        public class MappingProfile
        {
            [Fact]
            public void IsValid()
            {
                // Arrange
                AddBucket.MappingProfile profile = new AddBucket.MappingProfile();
                MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(profile));

                // Act and Assert
                configuration.AssertConfigurationIsValid();
            }

            [Fact]
            public void When_MappingCommandToBucketWithEmptyStringProperties_Expect_NullMappedToStringProperties()
            {
                // Arrange
                Bucket bucket = new Bucket { Name = "abc", Description = "xyz" };
                AddBucket.Command command = new AddBucket.Command { Name = string.Empty, Description = string.Empty };

                MapperConfiguration configuration = new MapperConfiguration(config =>
                    config.AddProfile(new AddBucket.MappingProfile()));

                // Act
                bucket = configuration.CreateMapper().Map<AddBucket.Command, Bucket>(command, bucket);

                // Assert
                Assert.Null(bucket.Name);
                Assert.Null(bucket.Description);
            }
        }
    }
}
