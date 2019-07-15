namespace Scaffold.WebApi.UnitTests.Views.MappingProfiles
{
    using AutoMapper;
    using Scaffold.Application.Features.Item;
    using Scaffold.WebApi.Views;
    using Scaffold.WebApi.Views.MappingProfiles;
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

        [Fact]
        public void When_MappingItemToAddItemCommandWithEmptyStringProperties_Expect_NullMappedToStringProperties()
        {
            // Arrange
            Item item = new Item { Name = string.Empty, Description = string.Empty };
            AddItem.Command command = new AddItem.Command { Name = "abc", Description = "xyz" };

            MapperConfiguration configuration = new MapperConfiguration(config =>
                config.AddProfile(new ItemMappingProfile()));

            // Act
            command = configuration.CreateMapper().Map(item, command);

            // Assert
            Assert.Null(command.Name);
            Assert.Null(command.Description);
        }

        [Fact]
        public void When_MappingItemToUpdateItemCommandWithEmptyStringProperties_Expect_NullMappedToStringProperties()
        {
            // Arrange
            Item item = new Item { Name = string.Empty, Description = string.Empty };
            UpdateItem.Command command = new UpdateItem.Command { Name = "abc", Description = "xyz" };

            MapperConfiguration configuration = new MapperConfiguration(config =>
                config.AddProfile(new ItemMappingProfile()));

            // Act
            command = configuration.CreateMapper().Map(item, command);

            // Assert
            Assert.Null(command.Name);
            Assert.Null(command.Description);
        }
    }
}
