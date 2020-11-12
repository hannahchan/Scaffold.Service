namespace Scaffold.WebApi.UnitTests.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Scaffold.WebApi.Controllers;
    using Xunit;

    public class ErrorControllerUnitTests
    {
        [Fact]
        public void When_InvokingError_Expect_ProblemDetails()
        {
            // Arrange
            ErrorController controller = new ErrorController
            {
                ProblemDetailsFactory = new MockProblemDetailsFactory(),
            };

            // Act
            ActionResult result = controller.Error();

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.IsType<ProblemDetails>(objectResult.Value);
        }

        private class MockProblemDetailsFactory : ProblemDetailsFactory
        {
            public override ProblemDetails CreateProblemDetails(
                HttpContext httpContext,
                int? statusCode = null,
                string title = null,
                string type = null,
                string detail = null,
                string instance = null)
            {
                return new ProblemDetails();
            }

            public override ValidationProblemDetails CreateValidationProblemDetails(
                HttpContext httpContext,
                ModelStateDictionary modelStateDictionary,
                int? statusCode = null,
                string title = null,
                string type = null,
                string detail = null,
                string instance = null)
            {
                return new ValidationProblemDetails();
            }
        }
    }
}
