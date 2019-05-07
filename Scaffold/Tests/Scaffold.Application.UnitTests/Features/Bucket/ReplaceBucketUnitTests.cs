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
    using Scaffold.Data;
    using Scaffold.Data.Repositories;
    using Scaffold.Domain.Entities;
    using Scaffold.Domain.Exceptions;
    using Xunit;

    public class ReplaceBucketUnitTests
    {
        private readonly IBucketRepository repository;

        public ReplaceBucketUnitTests()
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
                ReplaceBucket.Validator validator = new ReplaceBucket.Validator();

                // Act and Assert
                validator.ShouldNotHaveValidationErrorFor(command => command.Id, new Random().Next(int.MaxValue));
                validator.ShouldNotHaveValidationErrorFor(command => command.Name, Guid.NewGuid().ToString());
            }

            [Fact]
            public void ShouldHaveValidationErrorFor()
            {
                // Arrange
                ReplaceBucket.Validator validator = new ReplaceBucket.Validator();

                // Act and Assert
                validator.ShouldHaveValidationErrorFor(command => command.Id, default(int));
                validator.ShouldHaveValidationErrorFor(command => command.Name, string.Empty);
                validator.ShouldHaveValidationErrorFor(command => command.Name, value: null);
            }
        }

        public class Handler : ReplaceBucketUnitTests
        {
            [Fact]
            public async Task When_ReplacingBucket_Expect_ReplacedBucked()
            {
                // Arrange
                Bucket bucket = new Bucket
                {
                    Name = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                    Size = new Random().Next(int.MaxValue),
                };

                await this.repository.AddAsync(bucket);

                ReplaceBucket.Command command = new ReplaceBucket.Command
                {
                    Id = bucket.Id,
                    Name = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                    Size = new Random().Next(int.MaxValue),
                };

                ReplaceBucket.Handler handler = new ReplaceBucket.Handler(this.repository);

                // Act
                ReplaceBucket.Response response = await handler.Handle(command, default(CancellationToken));

                // Assert
                Assert.False(response.Created);
                Assert.True(response.Replaced);
                Assert.Equal(bucket.Id, response.Bucket.Id);
                Assert.Equal(bucket.Name, response.Bucket.Name);
                Assert.Equal(bucket.Description, response.Bucket.Description);
                Assert.Equal(bucket.Size, response.Bucket.Size);
            }

            [Fact]
            public async Task When_ReplacingNonExistingBucket_Expect_NewBucket()
            {
                // Arrange
                ReplaceBucket.Command command = new ReplaceBucket.Command
                {
                    Id = new Random().Next(int.MaxValue),
                    Name = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                    Size = new Random().Next(int.MaxValue),
                };

                ReplaceBucket.Handler handler = new ReplaceBucket.Handler(this.repository);

                // Act
                ReplaceBucket.Response response = await handler.Handle(command, default(CancellationToken));

                // Assert
                Assert.True(response.Created);
                Assert.False(response.Replaced);
                Assert.Equal(command.Id, response.Bucket.Id);
                Assert.Equal(command.Name, response.Bucket.Name);
                Assert.Equal(command.Description, response.Bucket.Description);
                Assert.Equal(command.Size, response.Bucket.Size);
            }

            [Fact]
            public async Task When_ReplacingBucketWithInvalidCommand_Expect_ValidationException()
            {
                // Arrange
                ReplaceBucket.Command command = new ReplaceBucket.Command { Name = string.Empty };
                ReplaceBucket.Handler handler = new ReplaceBucket.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default(CancellationToken)));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ValidationException>(exception);
            }

            [Fact]
            public async Task When_ReplacingBucketResultingInDomainConflict_Expect_DomainException()
            {
                // Arrange
                Bucket bucket = new Bucket();
                bucket.AddItem(new Item());
                await this.repository.AddAsync(bucket);

                ReplaceBucket.Command command = new ReplaceBucket.Command
                {
                    Id = bucket.Id,
                    Name = Guid.NewGuid().ToString(),
                    Size = 0,
                };

                ReplaceBucket.Handler handler = new ReplaceBucket.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(command, default(CancellationToken)));

                // Assert
                Assert.NotNull(exception);
                Assert.IsAssignableFrom<DomainException>(exception);
            }

            [Fact]
            public async Task When_ReplacingNonExistingBucketResultingInDomainConflict_Expect_DomainException()
            {
                // Arrange
                ReplaceBucket.Command command = new ReplaceBucket.Command
                {
                    Id = new Random().Next(int.MaxValue),
                    Name = Guid.NewGuid().ToString(),
                    Size = -1,
                };

                ReplaceBucket.Handler handler = new ReplaceBucket.Handler(this.repository);

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
                ReplaceBucket.MappingProfile profile = new ReplaceBucket.MappingProfile();
                MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(profile));

                // Act and Assert
                configuration.AssertConfigurationIsValid();
            }

            [Fact]
            public void When_MappingCommandToBucketWithEmptyStringProperties_Expect_NullMappedToStringProperties()
            {
                // Arrange
                Bucket bucket = new Bucket { Name = "abc", Description = "xyz" };
                ReplaceBucket.Command command = new ReplaceBucket.Command { Name = string.Empty, Description = string.Empty };

                MapperConfiguration configuration = new MapperConfiguration(config =>
                    config.AddProfile(new ReplaceBucket.MappingProfile()));

                // Act
                bucket = configuration.CreateMapper().Map<ReplaceBucket.Command, Bucket>(command, bucket);

                // Assert
                Assert.Null(bucket.Name);
                Assert.Null(bucket.Description);
            }
        }
    }
}
