namespace Scaffold.WebApi.UnitTests.Views.MappingProfiles
{
    using AutoMapper;
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
    }
}
