namespace Scaffold.WebApi.UnitTests.Views.MappingProfiles
{
    using AutoMapper;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.WebApi.Views;
    using Scaffold.WebApi.Views.MappingProfiles;
    using Xunit;

    public class BucketMappingProfileUnitTests
    {
        [Fact]
        public void BucketMappingProfile_IsValid()
        {
            // Arrange
            BucketMappingProfile profile = new BucketMappingProfile();
            MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(profile));

            // Act and Assert
            configuration.AssertConfigurationIsValid();
        }

        [Fact]
        public void When_MappingBucketToAddBucketCommandWithEmptyStringProperties_Expect_NullMappedToStringProperties()
        {
            // Arrange
            Bucket bucket = new Bucket { Name = string.Empty, Description = string.Empty };
            AddBucket.Command command = new AddBucket.Command { Name = "abc", Description = "xyz" };

            MapperConfiguration configuration = new MapperConfiguration(config =>
                config.AddProfile(new BucketMappingProfile()));

            // Act
            command = configuration.CreateMapper().Map(bucket, command);

            // Assert
            Assert.Null(command.Name);
            Assert.Null(command.Description);
        }

        [Fact]
        public void When_MappingBucketToUpdateBucketCommandWithEmptyStringProperties_Expect_NullMappedToStringProperties()
        {
            // Arrange
            Bucket bucket = new Bucket { Name = string.Empty, Description = string.Empty };
            UpdateBucket.Command command = new UpdateBucket.Command { Name = "abc", Description = "xyz" };

            MapperConfiguration configuration = new MapperConfiguration(config =>
                config.AddProfile(new BucketMappingProfile()));

            // Act
            command = configuration.CreateMapper().Map(bucket, command);

            // Assert
            Assert.Null(command.Name);
            Assert.Null(command.Description);
        }
    }
}
