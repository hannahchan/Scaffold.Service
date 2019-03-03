namespace Scaffold.WebApi.UnitTests.Services
{
    using Scaffold.WebApi.Services;
    using Xunit;

    public class RequestTracingServiceUnitTests
    {
        [Fact]
        public void When_InstantiatingNewRequestTracingService_Expect_CorrelationIdNull()
        {
            // Arrange
            RequestTracingService service;

            // Act
            service = new RequestTracingService();

            // Assert
            Assert.Null(service.CorrelationId);
        }
    }
}
