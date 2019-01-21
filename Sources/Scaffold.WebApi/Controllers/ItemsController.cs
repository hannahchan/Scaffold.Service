namespace Scaffold.WebApi.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/buckets/{bucketId}/[controller]")]
    public class ItemsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Item 1", "Item 2" };
        }

        [HttpGet("{itemId}")]
        public ActionResult<string> Get(int itemId)
        {
            return $"Item {itemId}";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPatch("{itemId}")]
        public void Patch(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{itemId}")]
        public void Delete(int id)
        {
        }
    }
}
