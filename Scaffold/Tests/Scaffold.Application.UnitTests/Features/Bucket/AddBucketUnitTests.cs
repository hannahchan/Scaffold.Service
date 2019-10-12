namespace Scaffold.Application.UnitTests.Features.Bucket
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using FluentValidation.TestHelper;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Domain.Base;
    using Scaffold.Repositories.EntityFrameworkCore;
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

        public class Response
        {
            [Fact]
            public void When_InstantiatingResponseWithBucket_Expect_ResponseWithBucket()
            {
                // Arrange
                Bucket bucket = new Bucket();

                // Act
                AddBucket.Response response = new AddBucket.Response(bucket);

                // Assert
                Assert.Equal(bucket, response.Bucket);
            }

            [Fact]
            public void When_InstantiatingResponseWithNull_Expect_ArgumentNullException()
            {
                // Act
                Exception exception = Record.Exception(() => new AddBucket.Response(null!));

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
            public async Task When_AddingBucket_Expect_AddedBucket()
            {
                // Arrange
                AddBucket.Command command = new AddBucket.Command
                {
                    Name = Guid.NewGuid().ToString(),
                };

                AddBucket.Handler handler = new AddBucket.Handler(this.repository);

                // Act
                AddBucket.Response response = await handler.Handle(command, default);

                // Assert
                Assert.NotEqual(default, response.Bucket.Id);
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
                    handler.Handle(command, default));

                // Assert
                Assert.IsType<ValidationException>(exception);
            }

            [Fact]
            public async Task When_AddingBucketResultingInDomainConflict_Expect_DomainException()
            {
                // Arrange
                AddBucket.Command command = new AddBucket.Command
                {
                    Name = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                    Size = -1,
                };
                AddBucket.Handler handler = new AddBucket.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default));

                // Assert
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
        }
    }
}
