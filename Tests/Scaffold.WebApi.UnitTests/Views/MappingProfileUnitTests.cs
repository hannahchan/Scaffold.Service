namespace Scaffold.WebApi.UnitTests.Views
{
    using AutoMapper;
    using Scaffold.WebApi.Views.MappingProfiles;
    using Xunit;

    public class MappingProfileUnitTests
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
        public void ItemMappingProfile_IsValid()
        {
            // Arrange
            ItemMappingProfile profile = new ItemMappingProfile();
            MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(profile));

            // Act and Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
