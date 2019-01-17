namespace Scaffold.Application.UnitTests.Behaviors
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Behavior;
    using Scaffold.Application.Context;
    using Xunit;

    public class RequestIdBehaviorUnitTests
    {
        [Fact]
        public async Task When_HandlingApplicationRequest_Expect_RequestIdInResponse()
        {
            // Arrange
            RequestIdBehavior<Request, Response> behavior = new RequestIdBehavior<Request, Response>();

            Request request = new Request { RequestId = Guid.NewGuid().ToString() };
            RequestHandlerDelegate<Response> next = new RequestHandlerDelegate<Response>(this.ReturnResponse);

            // Act
            Response response = await behavior.Handle(request, default(CancellationToken), next);

            // Assert
            Assert.Equal(request.RequestId, response.RequestId);
        }

        private Task<Response> ReturnResponse() => Task.FromResult(new Response());

        private class Request : ApplicationRequest
        {
        }

        private class Response : ApplicationResponse
        {
        }
    }
}
