namespace Scaffold.Application.UnitTests.Context
{
    using System;
    using Scaffold.Application.Context;
    using Xunit;

    public class ApplicationRequestUnitTests
    {
        [Fact]
        public void When_InstantiatingNewRequest_Expect_RequestIdEmpty()
        {
            // Arrange
            Request request;

            // Act
            request = new Request();

            // Assert
            Assert.Equal(string.Empty, request.RequestId);
        }

        private class Request : ApplicationRequest
        {
        }
    }
}
