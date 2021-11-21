namespace Scaffold.WebApi.UnitTests.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Scaffold.WebApi.Extensions;
using Scaffold.WebApi.HttpMessageHandlers;
using Xunit;

public class HttpClientBuilderExtensionsUnitTests
{
    public class AddRequestLogging
    {
        [Fact]
        public void When_AddingRequestLogging_Expect_ServicesAdded()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            IHttpClientBuilder builder = new MockHttpClientBuilder(services);

            // Act
            builder.AddRequestLogging();

            // Assert
            Assert.Collection(
                services,
                serviceDescriptor =>
                {
                    Assert.Equal(ServiceLifetime.Transient, serviceDescriptor.Lifetime);
                    Assert.Equal(typeof(RequestLoggingHttpMessageHandler), serviceDescriptor.ServiceType);
                    Assert.Equal(typeof(RequestLoggingHttpMessageHandler), serviceDescriptor.ImplementationType);
                },
                serviceDescriptor =>
                {
                    Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                    Assert.Equal(typeof(IOptions<>), serviceDescriptor.ServiceType);
                },
                serviceDescriptor =>
                {
                    Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
                    Assert.Equal(typeof(IOptionsSnapshot<>), serviceDescriptor.ServiceType);
                    Assert.Equal(typeof(OptionsManager<>), serviceDescriptor.ImplementationType);
                },
                serviceDescriptor =>
                {
                    Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                    Assert.Equal(typeof(IOptionsMonitor<>), serviceDescriptor.ServiceType);
                    Assert.Equal(typeof(OptionsMonitor<>), serviceDescriptor.ImplementationType);
                },
                serviceDescriptor =>
                {
                    Assert.Equal(ServiceLifetime.Transient, serviceDescriptor.Lifetime);
                    Assert.Equal(typeof(IOptionsFactory<>), serviceDescriptor.ServiceType);
                    Assert.Equal(typeof(OptionsFactory<>), serviceDescriptor.ImplementationType);
                },
                serviceDescriptor =>
                {
                    Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                    Assert.Equal(typeof(IOptionsMonitorCache<>), serviceDescriptor.ServiceType);
                    Assert.Equal(typeof(OptionsCache<>), serviceDescriptor.ImplementationType);
                },
                serviceDescriptor =>
                {
                    Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                    Assert.Equal(typeof(IConfigureOptions<HttpClientFactoryOptions>), serviceDescriptor.ServiceType);
                    Assert.Null(serviceDescriptor.ImplementationType);
                });
        }
    }

    private class MockHttpClientBuilder : IHttpClientBuilder
    {
        public MockHttpClientBuilder(IServiceCollection services)
        {
            this.Services = services;
        }

        public string Name => "MockHttpClientBuilder";

        public IServiceCollection Services { get; }
    }
}
