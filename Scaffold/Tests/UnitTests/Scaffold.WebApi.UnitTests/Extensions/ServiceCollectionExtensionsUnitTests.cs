namespace Scaffold.WebApi.UnitTests.Extensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Scaffold.WebApi.Extensions;
    using Xunit;

    public class ServiceCollectionExtensionsUnitTests
    {
        [Fact]
        public void When_AddingApiDocumentationWithNullServices_Expect_ArgumentNullException()
        {
            // Arrange
            IServiceCollection services = null;

            // Act
            Exception exception = Record.Exception(() => services.AddApiDocumentation());

            // Assert
            ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("services", argumentNullException.ParamName);
        }

        [Fact]
        public void When_AddingHttpClientsWithNullServices_Expect_ArgumentNullException()
        {
            // Arrange
            IServiceCollection services = null;

            // Act
            Exception exception = Record.Exception(() => services.AddHttpClients());

            // Assert
            ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("services", argumentNullException.ParamName);
        }

        [Fact]
        public void When_AddingOptionsWithNullServices_Expect_ArgumentNullException()
        {
            // Arrange
            IServiceCollection services = null;

            // Act
            Exception exception = Record.Exception(() =>
                services.AddOptions(new ConfigurationRoot(new List<IConfigurationProvider>())));

            // Assert
            ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("services", argumentNullException.ParamName);
        }

        [Fact]
        public void When_AddingOptionsWithNullConfiguration_Expect_ArgumentNullException()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            Exception exception = Record.Exception(() => services.AddOptions(null));

            // Assert
            ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("config", argumentNullException.ParamName);
        }

        [Fact]
        public void When_AddingRepositoriesWithNullServices_Expect_ArgumentNullException()
        {
            // Arrange
            IServiceCollection services = null;

            // Act
            Exception exception = Record.Exception(() =>
                services.AddRepositories(new ConfigurationRoot(new List<IConfigurationProvider>())));

            // Assert
            ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("services", argumentNullException.ParamName);
        }

        [Fact]
        public void When_AddingRepositoriesWithNullConfiguration_Expect_ArgumentNullException()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            Exception exception = Record.Exception(() => services.AddRepositories(null));

            // Assert
            ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("config", argumentNullException.ParamName);
        }

        [Fact]
        public void When_AddingServicesWithNullServices_Expect_ArgumentNullException()
        {
            // Arrange
            IServiceCollection services = null;

            // Act
            Exception exception = Record.Exception(() => services.AddServices());

            // Assert
            ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("services", argumentNullException.ParamName);
        }
    }
}
