namespace Scaffold.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Scaffold.Application.Exceptions;
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

        /// <summary>Retrieves a list of buckets.</summary>
        /// <returns>A list of Bucket objects.</returns>
        /// <response code="default">Problem Details (RFC 7807) Response</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IList<Bucket>>> Get()
        {
            GetBuckets.Query query = new GetBuckets.Query();
            GetBuckets.Response response = await this.mediator.Send(query);

            return this.mapper.Map<List<Bucket>>(response.Buckets);
        }

        /// <summary>Retrieves a bucket.</summary>
        /// <param name="bucketId">The Id. of the Bucket object to be retrieved.</param>
        /// <returns>The specified Bucket object.</returns>
        /// <response code="default">Problem Details (RFC 7807) Response</response>
        [HttpGet("{bucketId}", Name = "GetBucket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Bucket>> Get(int bucketId)
        {
            GetBucket.Query query = new GetBucket.Query { Id = bucketId };
            GetBucket.Response response = await this.mediator.Send(query);

            if (response.Bucket == null)
            {
                throw new BucketNotFoundException(bucketId);
            }

            return this.mapper.Map<Bucket>(response.Bucket);
        }

        /// <summary>Creates a bucket.</summary>
        /// <param name="bucket">A complete or partial set of key-value pairs to create the Bucket object with.</param>
        /// <returns>The created Bucket object.</returns>
        /// <response code="default">Problem Details (RFC 7807) Response</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Bucket>> Post([FromBody] Bucket bucket)
        {
            AddBucket.Command command = this.mapper.Map<AddBucket.Command>(bucket);
            AddBucket.Response response = await this.mediator.Send(command);

            bucket = this.mapper.Map<Bucket>(response.Bucket);

            return this.CreatedAtRoute("GetBucket", new { bucketId = bucket.Id }, bucket);
        }

        /// <summary>Updates a bucket.</summary>
        /// <param name="bucketId">The Id. of the Bucket object to be updated.</param>
        /// <param name="bucket">A complete or partial set of key-value pairs to update the Bucket object with.</param>
        /// <returns>The updated Bucket object.</returns>
        /// <response code="default">Problem Details (RFC 7807) Response</response>
        [HttpPatch("{bucketId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Bucket>> Patch(int bucketId, [FromBody] Bucket bucket)
        {
            UpdateBucket.Command command = this.mapper.Map<UpdateBucket.Command>(bucket);
            command.Id = bucketId;

            UpdateBucket.Response response = await this.mediator.Send(command);

            return this.mapper.Map<Bucket>(response.Bucket);
        }

        /// <summary>Deletes a bucket.</summary>
        /// <param name="bucketId">The Id. of the Bucket object to be deleted.</param>
        /// <returns>A "No Content (204)" HTTP status response.</returns>
        /// <response code="default">Problem Details (RFC 7807) Response</response>
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
