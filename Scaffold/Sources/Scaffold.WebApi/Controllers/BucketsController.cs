namespace Scaffold.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.WebApi.Views;

    [ApiController]
    [Route("api/[controller]")]
    public class BucketsController : ControllerBase
    {
        private readonly IMapper mapper;

        private readonly IMediator mediator;

        public BucketsController(IMapper mapper, IMediator mediator)
        {
            this.mapper = mapper;
            this.mediator = mediator;
        }

        /// <summary>Creates a bucket.</summary>
        /// <param name="bucket">A complete or partial set of key-value pairs to create the Bucket object with.</param>
        /// <returns>The created Bucket object.</returns>
        /// <response code="201">Bucket created successfully.</response>
        /// <response code="default">Problem Details (RFC 7807) Response.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Bucket))]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Post([FromBody] Bucket bucket)
        {
            AddBucket.Command command = this.mapper.Map<AddBucket.Command>(bucket);
            AddBucket.Response response = await this.mediator.Send(command);

            bucket = this.mapper.Map<Bucket>(response.Bucket);

            return this.CreatedAtRoute("GetBucket", new { bucketId = bucket.Id }, bucket);
        }

        /// <summary>Retrieves a list of buckets.</summary>
        /// <param name="limit">The maximun number of buckets to return from the result set. Defaults to 10.</param>
        /// <param name="offset">The number of buckets to omit from the start of the result set.</param>
        /// <returns>A list of Bucket objects.</returns>
        /// <response code="200">Buckets retrieved successfully.</response>
        /// <response code="default">Problem Details (RFC 7807) Response.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IList<Bucket>> Get([FromQuery]int? limit, [FromQuery]int? offset)
        {
            GetBuckets.Query query = new GetBuckets.Query { Limit = limit ?? 10, Offset = offset };
            GetBuckets.Response response = await this.mediator.Send(query);

            return this.mapper.Map<List<Bucket>>(response.Buckets);
        }

        /// <summary>Retrieves a bucket.</summary>
        /// <param name="bucketId">The Id. of the Bucket object to be retrieved.</param>
        /// <returns>The specified Bucket object.</returns>
        /// <response code="200">Bucket retrieved successfully.</response>
        /// <response code="default">Problem Details (RFC 7807) Response.</response>
        [HttpGet("{bucketId}", Name = "GetBucket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<Bucket> Get(int bucketId)
        {
            GetBucket.Query query = new GetBucket.Query { Id = bucketId };
            GetBucket.Response response = await this.mediator.Send(query);

            return this.mapper.Map<Bucket>(response.Bucket);
        }

        /// <summary>Updates a bucket or creates one if the specified one does not exist.</summary>
        /// <param name="bucketId">The Id. of the Bucket object to be created or updated.</param>
        /// <param name="bucket">A complete set of key-value pairs to create or update the Bucket object with.</param>
        /// <returns>The created or updated Bucket object.</returns>
        /// <response code="200">Bucket updated successfully.</response>
        /// <response code="201">Bucket created successfully.</response>
        /// <response code="default">Problem Details (RFC 7807) Response.</response>
        [HttpPut("{bucketId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Bucket))]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Bucket>> Put(int bucketId, [FromBody] Bucket bucket)
        {
            UpdateBucket.Command command = this.mapper.Map<UpdateBucket.Command>(bucket);
            command.Id = bucketId;

            UpdateBucket.Response response = await this.mediator.Send(command);
            bucket = this.mapper.Map<Bucket>(response.Bucket);

            if (response.Created)
            {
                return this.CreatedAtRoute("GetBucket", new { bucketId = bucket.Id }, bucket);
            }

            return bucket;
        }

        /// <summary>Deletes a bucket.</summary>
        /// <param name="bucketId">The Id. of the Bucket object to be deleted.</param>
        /// <returns>A "No Content (204)" HTTP status response.</returns>
        /// <response code="204">Bucket deleted successfully.</response>
        /// <response code="default">Problem Details (RFC 7807) Response.</response>
        [HttpDelete("{bucketId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Delete(int bucketId)
        {
            await this.mediator.Send(new RemoveBucket.Command { Id = bucketId });
            return this.NoContent();
        }
    }
}
