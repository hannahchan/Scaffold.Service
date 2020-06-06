namespace Scaffold.WebApi.UnitTests.Views
{
    using AutoMapper;
    using Scaffold.WebApi.Views.Item;
    using Xunit;

    public class ItemMappingProfileUnitTests
    {
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
