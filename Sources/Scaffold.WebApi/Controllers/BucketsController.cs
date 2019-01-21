namespace Scaffold.WebApi.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class BucketsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Bucket 1", "Bucket 2" };
        }

        [HttpGet("{bucketId}")]
        public ActionResult<string> Get(int bucketId)
        {
            return $"Bucket {bucketId}";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPatch("{bucketId}")]
        public void Patch(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{bucketId}")]
        public void Delete(int id)
        {
        }
    }
}
