namespace Scaffold.WebApi.UnitTests.Services
{
    using Scaffold.WebApi.Services;
    using Xunit;

    public class RequestIdServiceUnitTests
    {
        [Fact]
        public void When_InstantiatingNewRequestIdService_Expect_RequestIdNull()
        {
            // Arrange
            RequestIdService service;

            // Act
            service = new RequestIdService();

            // Assert
            Assert.Null(service.RequestId);
        }
    }
}
