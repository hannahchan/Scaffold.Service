namespace Scaffold.Application.UnitTests.Features.Bucket
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using FluentValidation.TestHelper;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.Application.Interfaces;
    using Scaffold.Application.Repositories;
    using Scaffold.Data;
    using Scaffold.Domain.Entities;
    using Scaffold.Domain.Exceptions;
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
                validator.ShouldNotHaveValidationErrorFor(command => command.Name, value: null);
            }

            [Fact]
            public void ShouldHaveValidationErrorFor()
            {
                // Arrange
                UpdateBucket.Validator validator = new UpdateBucket.Validator();

                // Act and Assert
                validator.ShouldHaveValidationErrorFor(command => command.Id, default(int));
                validator.ShouldHaveValidationErrorFor(command => command.Name, string.Empty);
            }
        }

        public class Handler : UpdateBucketUnitTests
        {
            [Fact]
            public async Task When_UpdatingBucket_Expect_Updated()
            {
                // Arrange
                Bucket bucket = new Bucket();
                await this.repository.AddAsync(bucket);

                string newValue = Guid.NewGuid().ToString();

                UpdateBucket.Command command = new UpdateBucket.Command { Id = bucket.Id, Name = newValue };
                UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository);

                // Act
                UpdateBucket.Response response = await handler.Handle(command, default(CancellationToken));

                // Assert
                Assert.Equal(newValue, response.Bucket.Name);
            }

            [Fact]
            public async Task When_UpdatingNonExistingBucket_Expect_BucketNotFoundException()
            {
                // Arrange
                string newValue = Guid.NewGuid().ToString();

                UpdateBucket.Command command = new UpdateBucket.Command
                {
                    Id = new Random().Next(int.MaxValue),
                    Name = newValue
                };

                UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default(CancellationToken)));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<BucketNotFoundException>(exception);
            }

            [Fact]
            public async Task When_UpdatingBucketWithInvalidCommand_Expect_ValidationException()
            {
                // Arrange
                Bucket bucket = new Bucket();
                await this.repository.AddAsync(bucket);

                UpdateBucket.Command command = new UpdateBucket.Command { Id = bucket.Id, Name = string.Empty };
                UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default(CancellationToken)));

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

                UpdateBucket.Command command = new UpdateBucket.Command { Id = bucket.Id, Size = 0 };
                UpdateBucket.Handler handler = new UpdateBucket.Handler(this.repository);

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
                UpdateBucket.MappingProfile profile = new UpdateBucket.MappingProfile();
                MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(profile));

                // Act and Assert
                configuration.AssertConfigurationIsValid();
            }

            [Fact]
            public void When_MappingCommandToBucketWithOnlyNameNotNull_Expect_NameMapped()
            {
                // Arrange
                Bucket bucket = new Bucket { Name = "abc", Description = "xyz" };
                UpdateBucket.Command command = new UpdateBucket.Command { Name = "def" };

                MapperConfiguration configuration = new MapperConfiguration(config =>
                    config.AddProfile(new UpdateBucket.MappingProfile()));

                // Act
                bucket = configuration.CreateMapper().Map<UpdateBucket.Command, Bucket>(command, bucket);

                // Assert
                Assert.Equal("def", bucket.Name);
                Assert.Equal("xyz", bucket.Description);
            }

            [Fact]
            public void When_MappingCommandToBucketWithDescriptionNotNull_Expect_DescriptionMapped()
            {
                // Arrange
                Bucket bucket = new Bucket { Name = "abc", Description = "xyz" };
                UpdateBucket.Command command = new UpdateBucket.Command { Description = "uvw" };

                MapperConfiguration configuration = new MapperConfiguration(config =>
                    config.AddProfile(new UpdateBucket.MappingProfile()));

                // Act
                bucket = configuration.CreateMapper().Map<UpdateBucket.Command, Bucket>(command, bucket);

                // Assert
                Assert.Equal("abc", bucket.Name);
                Assert.Equal("uvw", bucket.Description);
            }

            [Fact]
            public void When_MappingCommandToBucketWithSizeNotNull_Expect_SizeMapped()
            {
                // Arrange
                Bucket bucket = new Bucket { Name = "abc", Description = "xyz" };
                UpdateBucket.Command command = new UpdateBucket.Command { Size = new Random().Next(int.MaxValue) };

                MapperConfiguration configuration = new MapperConfiguration(config =>
                    config.AddProfile(new UpdateBucket.MappingProfile()));

                // Act
                bucket = configuration.CreateMapper().Map<UpdateBucket.Command, Bucket>(command, bucket);

                // Assert
                Assert.Equal("abc", bucket.Name);
                Assert.Equal("xyz", bucket.Description);
                Assert.Equal(command.Size, bucket.Size);
            }

            [Fact]
            public void When_MappingCommandToBucketWithEmptyStringProperties_Expect_NullMappedToStringProperties()
            {
                // Arrange
                Bucket bucket = new Bucket { Name = "abc", Description = "xyz" };
                UpdateBucket.Command command = new UpdateBucket.Command { Name = string.Empty, Description = string.Empty };

                MapperConfiguration configuration = new MapperConfiguration(config =>
                    config.AddProfile(new UpdateBucket.MappingProfile()));

                // Act
                bucket = configuration.CreateMapper().Map<UpdateBucket.Command, Bucket>(command, bucket);

                // Assert
                Assert.Null(bucket.Name);
                Assert.Null(bucket.Description);
            }
        }
    }
}
