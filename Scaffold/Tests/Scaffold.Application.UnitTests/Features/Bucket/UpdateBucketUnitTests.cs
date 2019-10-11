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

    public class UpdateBucketUnitTests
    {
        private readonly IBucketRepository repository;

        public UpdateBucketUnitTests()
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
                UpdateBucket.Response response = new UpdateBucket.Response(bucket);

                // Assert
                Assert.Equal(bucket, response.Bucket);
                Assert.False(response.Created);
                Assert.True(response.Updated);
            }

            [Fact]
            public void When_InstantiatingResponseWithCreatedSetToTrue_Expect_CreatedTrue()
            {
                // Arrange
                Bucket bucket = new Bucket();

                // Act
                UpdateBucket.Response response = new UpdateBucket.Response(bucket, true);

                // Assert
                Assert.Equal(bucket, response.Bucket);
                Assert.True(response.Created);
                Assert.False(response.Updated);
            }

            [Fact]
            public void When_InstantiatingResponseWithCreatedSetToFalse_Expect_CreatedFalse()
            {
                // Arrange
                Bucket bucket = new Bucket();

                // Act
                UpdateBucket.Response response = new UpdateBucket.Response(bucket, false);

                // Assert
                Assert.Equal(bucket, response.Bucket);
                Assert.False(response.Created);
                Assert.True(response.Updated);
            }

            [Fact]
            public void When_InstantiatingResponseWithNull_Expect_ArgumentNullException()
            {
                // Act
                Exception exception = Record.Exception(() => new UpdateBucket.Response(null!));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
            }
        }

        public class Validator
        {
            [Fact]
            public void ShouldNotHaveValidationErrorFor()
            {
                // Arrange
                UpdateBucket.Validator validator = new UpdateBucket.Validator();

                // Act and Assert
                validator.ShouldNotHaveValidationErrorFor(command => command.Id, new Random().Next(int.MaxValue));
                validator.ShouldNotHaveValidationErrorFor(command => command.Name, Guid.NewGuid().ToString());
            }

            [Fact]
            public void ShouldHaveValidationErrorFor()
            {
                // Arrange
                UpdateBucket.Validator validator = new UpdateBucket.Validator();

                // Act and Assert
                validator.ShouldHaveValidationErrorFor(command => command.Id, default(int));
                validator.ShouldHaveValidationErrorFor(command => command.Name, string.Empty);
                validator.ShouldHaveValidationErrorFor(command => command.Name, null as string);
            }
        }

        public class Handler : UpdateBucketUnitTests
        {
            [Fact]
            public async Task When_UpdatingBucket_Expect_UpdatedBucked()
            {
                // Arrange
                Bucket bucket = new Bucket
                {
                    Name = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                    Size = new Random().Next(int.MaxValue),
                };

                await this.repository.AddAsync(bucket);

                UpdateBucket.Command command = new UpdateBucket.Command
                {
                    Id = bucket.Id,
                    Name = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                    Size = new Random().Next(int.MaxValue),
                };

                UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository);

                // Act
                UpdateBucket.Response response = await handler.Handle(command, default);

                // Assert
                Assert.False(response.Created);
                Assert.True(response.Updated);
                Assert.Equal(bucket.Id, response.Bucket.Id);
                Assert.Equal(bucket.Name, response.Bucket.Name);
                Assert.Equal(bucket.Description, response.Bucket.Description);
                Assert.Equal(bucket.Size, response.Bucket.Size);
            }

            [Fact]
            public async Task When_UpdatingNonExistingBucket_Expect_NewBucket()
            {
                // Arrange
                UpdateBucket.Command command = new UpdateBucket.Command
                {
                    Id = new Random().Next(int.MaxValue),
                    Name = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                    Size = new Random().Next(int.MaxValue),
                };

                UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository);

                // Act
                UpdateBucket.Response response = await handler.Handle(command, default);

                // Assert
                Assert.True(response.Created);
                Assert.False(response.Updated);
                Assert.Equal(command.Id, response.Bucket.Id);
                Assert.Equal(command.Name, response.Bucket.Name);
                Assert.Equal(command.Description, response.Bucket.Description);
                Assert.Equal(command.Size, response.Bucket.Size);
            }

            [Fact]
            public async Task When_UpdatingBucketWithInvalidCommand_Expect_ValidationException()
            {
                // Arrange
                UpdateBucket.Command command = new UpdateBucket.Command { Name = string.Empty };
                UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ValidationException>(exception);
            }

            [Fact]
            public async Task When_UpdatingBucketResultingInDomainConflict_Expect_DomainException()
            {
                // Arrange
                Bucket bucket = new Bucket();
                bucket.AddItem(new Item());
                await this.repository.AddAsync(bucket);

                UpdateBucket.Command command = new UpdateBucket.Command
                {
                    Id = bucket.Id,
                    Name = Guid.NewGuid().ToString(),
                    Size = 0,
                };

                UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default));

                // Assert
                Assert.NotNull(exception);
                Assert.IsAssignableFrom<DomainException>(exception);
            }

            [Fact]
            public async Task When_UpdatingNonExistingBucketResultingInDomainConflict_Expect_DomainException()
            {
                // Arrange
                UpdateBucket.Command command = new UpdateBucket.Command
                {
                    Id = new Random().Next(int.MaxValue),
                    Name = Guid.NewGuid().ToString(),
                    Size = -1,
                };

                UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default));

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
                UpdateBucket.MappingProfile profile = new UpdateBucket.MappingProfile();
                MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(profile));

                // Act and Assert
                configuration.AssertConfigurationIsValid();
            }
        }
    }
}
