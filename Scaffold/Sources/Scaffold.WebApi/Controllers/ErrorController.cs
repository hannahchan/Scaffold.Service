namespace Scaffold.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("error")]
public class ErrorController : ControllerBase
{
    public ActionResult Error()
    {
        return this.Problem();
    }
}
