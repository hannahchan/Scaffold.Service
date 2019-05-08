namespace Scaffold.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Features.Item;
    using Scaffold.WebApi.Views;

    [ApiController]
    [Route("api/Buckets/{bucketId}/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IMapper mapper;

        private readonly IMediator mediator;

        public ItemsController(IMapper mapper, IMediator mediator)
        {
            this.mapper = mapper;
            this.mediator = mediator;
        }

        /// <summary>Creates an item in a bucket.</summary>
        /// <param name="bucketId">The Id. of the Bucket object to create the item in.</param>
        /// <param name="item">A complete or partial set of key-value pairs to create the Item object with.</param>
        /// <returns>The created Item object.</returns>
        /// <response code="201">Item created successfully.</response>
        /// <response code="default">Problem Details (RFC 7807) Response.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Item>> Post(int bucketId, [FromBody] Item item)
        {
            AddItem.Command command = this.mapper.Map<AddItem.Command>(item);
            command.BucketId = bucketId;

            AddItem.Response response = await this.mediator.Send(command);
            item = this.mapper.Map<Item>(response.Item);

            return this.CreatedAtRoute("GetItem", new { itemId = item.Id }, item);
        }

        /// <summary>Retrieves a list of items from a bucket.</summary>
        /// <param name="bucketId">The Id. of the Bucket object to retrieve the items from.</param>
        /// <returns>A list of Item objects.</returns>
        /// <response code="200">Items retrieved successfully.</response>
        /// <response code="default">Problem Details (RFC 7807) Response.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IList<Item>>> Get(int bucketId)
        {
            GetItems.Query query = new GetItems.Query { BucketId = bucketId };
            GetItems.Response response = await this.mediator.Send(query);

            return this.mapper.Map<List<Item>>(response.Items);
        }

        /// <summary>Retrieves an item from a bucket.</summary>
        /// <param name="bucketId">The Id. of the Bucket object to retrieve the item from.</param>
        /// <param name="itemId">The Id. of the Item object to be retrieved.</param>
        /// <returns>The specified Item object.</returns>
        /// <response code="200">Item retrieved successfully.</response>
        /// <response code="default">Problem Details (RFC 7807) Response.</response>
        [HttpGet("{itemId}", Name = "GetItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Item>> Get(int bucketId, int itemId)
        {
            GetItem.Query query = new GetItem.Query { BucketId = bucketId, ItemId = itemId };
            GetItem.Response response = await this.mediator.Send(query);

            if (response.Item == null)
            {
                throw new ItemNotFoundException(itemId);
            }

            return this.mapper.Map<Item>(response.Item);
        }

        /// <summary>Updates an item in a bucket or creates one if the specified one does not exist.</summary>
        /// <param name="bucketId">The Id. of the Bucket object to create or update the item in.</param>
        /// <param name="itemId">The Id. of the Item object to be created or updated.</param>
        /// <param name="item">A complete set of key-value pairs to create or update the Item object with.</param>
        /// <returns>The created or updated Item object.</returns>
        /// <response code="200">Item updated successfully.</response>
        /// <response code="201">Item created successfully.</response>
        /// <response code="default">Problem Details (RFC 7807) Response.</response>
        [HttpPut("{itemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Item>> Put(int bucketId, int itemId, [FromBody] Item item)
        {
            UpdateItem.Command command = this.mapper.Map<UpdateItem.Command>(item);
            command.BucketId = bucketId;
            command.ItemId = itemId;

            UpdateItem.Response response = await this.mediator.Send(command);
            item = this.mapper.Map<Item>(response.Item);

            if (response.Created)
            {
                return this.CreatedAtRoute("GetItem", new { itemId = item.Id }, item);
            }

            return item;
        }

        /// <summary>Deletes an item in a bucket.</summary>
        /// <param name="bucketId">The Id. of the Bucket object to delete the item from.</param>
        /// <param name="itemId">The Id. of the Item object to be deleted.</param>
        /// <returns>A "No Content (204)" HTTP status response.</returns>
        /// <response code="204">Item deleted successfully or did not exist.</response>
        /// <response code="default">Problem Details (RFC 7807) Response.</response>
        [HttpDelete("{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Delete(int bucketId, int itemId)
        {
            await this.mediator.Send(new RemoveItem.Command { BucketId = bucketId, ItemId = itemId });
            return this.NoContent();
        }
    }
}
