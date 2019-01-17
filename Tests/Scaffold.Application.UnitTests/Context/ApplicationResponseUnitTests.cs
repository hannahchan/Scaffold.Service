namespace Scaffold.Application.UnitTests.Context
{
    using System;
    using Scaffold.Application.Context;
    using Xunit;

    public class ApplicationResponseUnitTests
    {
        [Fact]
        public void When_InstantiatingNewResponse_Expect_RequestIdEmpty()
        {
            // Arrange
            Response response;

            // Act
            response = new Response();

            // Assert
            Assert.Equal(string.Empty, response.RequestId);
        }

        private class Response : ApplicationResponse
        {
        }
    }
}
