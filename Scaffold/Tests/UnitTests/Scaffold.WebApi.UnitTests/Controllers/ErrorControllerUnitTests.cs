namespace Scaffold.WebApi.UnitTests.Controllers;

using Microsoft.AspNetCore.Mvc;
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
            ProblemDetailsFactory = new Mock.ProblemDetailsFactory(),
        };

        // Act
        ActionResult result = controller.Error();

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
        Assert.IsType<ProblemDetails>(objectResult.Value);
    }
}
