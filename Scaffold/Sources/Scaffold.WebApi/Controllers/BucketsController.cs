﻿namespace Scaffold.WebApi.Controllers;

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scaffold.Application.Components.Bucket;
using Scaffold.Domain.Specifications;
using Scaffold.WebApi.Models.Bucket;
using Scaffold.WebApi.Models.Item;

[ApiController]
[Route("buckets")]
public class BucketsController : ControllerBase
{
    private readonly IMapper mapper;

    private readonly ISender sender;

    public BucketsController(IMapper mapper, ISender sender)
    {
        this.mapper = mapper;
        this.sender = sender;
    }

    /// <summary>Creates a bucket.</summary>
    /// <param name="requestBody">A complete or partial set of key-value pairs to create the Bucket object with.</param>
    /// <returns>The created Bucket object.</returns>
    /// <response code="201">Bucket created successfully.</response>
    /// <response code="default">Problem Details (RFC 7807).</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Bucket))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> AddBucket([FromBody] AddBucketRequestBody requestBody)
    {
        AddBucket.Command command = new AddBucket.Command(
            Name: requestBody.Name,
            Description: requestBody.Description,
            Size: requestBody.Size);

        AddBucket.Response response = await this.sender.Send(command);

        Bucket bucket = this.mapper.Map<Bucket>(response.Bucket);

        return this.CreatedAtRoute("GetBucket", new { bucketId = bucket.Id }, bucket);
    }

    /// <summary>Retrieves a list of buckets.</summary>
    /// <param name="requestQuery">The parameters for the request.</param>
    /// <returns>A list of Bucket objects.</returns>
    /// <response code="200">Buckets retrieved successfully.</response>
    /// <response code="default">Problem Details (RFC 7807).</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<IEnumerable<Bucket>> GetBuckets([FromQuery] GetBucketsRequestQuery requestQuery)
    {
        GetBuckets.Query query = new GetBuckets.Query(
            Predicate: new BucketSpecification.All().Expression,
            Limit: requestQuery.Limit,
            Offset: requestQuery.Offset,
            SortOrder: null);

        GetBuckets.Response response = await this.sender.Send(query);

        return this.mapper.Map<Bucket[]>(response.Buckets);
    }

    /// <summary>Retrieves a bucket.</summary>
    /// <param name="bucketId">The Id. of the Bucket object to be retrieved.</param>
    /// <returns>The specified Bucket object.</returns>
    /// <response code="200">Bucket retrieved successfully.</response>
    /// <response code="default">Problem Details (RFC 7807).</response>
    [HttpGet("{bucketId}", Name = "GetBucket")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<Bucket> GetBucket(int bucketId)
    {
        GetBucket.Response response = await this.sender.Send(new GetBucket.Query(bucketId));
        return this.mapper.Map<Bucket>(response.Bucket);
    }

    /// <summary>Updates a bucket or creates one if the specified one does not exist.</summary>
    /// <param name="bucketId">The Id. of the Bucket object to be created or updated.</param>
    /// <param name="requestBody">A complete set of key-value pairs to create or update the Bucket object with.</param>
    /// <returns>The created or updated Bucket object.</returns>
    /// <response code="200">Bucket updated successfully.</response>
    /// <response code="201">Bucket created successfully.</response>
    /// <response code="default">Problem Details (RFC 7807).</response>
    [HttpPut("{bucketId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Bucket))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<Bucket>> UpdateBucket(int bucketId, [FromBody] UpdateBucketRequestBody requestBody)
    {
        UpdateBucket.Command command = new UpdateBucket.Command(
            Id: bucketId,
            Name: requestBody.Name,
            Description: requestBody.Description,
            Size: requestBody.Size);

        UpdateBucket.Response response = await this.sender.Send(command);
        Bucket bucket = this.mapper.Map<Bucket>(response.Bucket);

        return response.Created ? this.CreatedAtRoute("GetBucket", new { bucketId }, bucket) : (ActionResult<Bucket>)bucket;
    }

    /// <summary>Deletes a bucket.</summary>
    /// <param name="bucketId">The Id. of the Bucket object to be deleted.</param>
    /// <returns>A "No Content (204)" HTTP status response.</returns>
    /// <response code="204">Bucket deleted successfully.</response>
    /// <response code="default">Problem Details (RFC 7807).</response>
    [HttpDelete("{bucketId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RemoveBucket(int bucketId)
    {
        await this.sender.Send(new RemoveBucket.Command(bucketId));
        return this.NoContent();
    }

    /// <summary>Creates an item in a bucket.</summary>
    /// <param name="bucketId">The Id. of the Bucket object to create the item in.</param>
    /// <param name="requestBody">A complete or partial set of key-value pairs to create the Item object with.</param>
    /// <returns>The created Item object.</returns>
    /// <response code="201">Item created successfully.</response>
    /// <response code="default">Problem Details (RFC 7807).</response>
    [HttpPost("{bucketId}/items")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Item))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> AddItem(int bucketId, [FromBody] AddItemRequestBody requestBody)
    {
        AddItem.Command command = new AddItem.Command(
            BucketId: bucketId,
            Name: requestBody.Name,
            Description: requestBody.Description);

        AddItem.Response response = await this.sender.Send(command);
        Item item = this.mapper.Map<Item>(response.Item);

        return this.CreatedAtRoute("GetItem", new { bucketId, itemId = item.Id }, item);
    }

    /// <summary>Retrieves a list of items from a bucket.</summary>
    /// <param name="bucketId">The Id. of the Bucket object to retrieve the items from.</param>
    /// <returns>A list of Item objects.</returns>
    /// <response code="200">Items retrieved successfully.</response>
    /// <response code="default">Problem Details (RFC 7807).</response>
    [HttpGet("{bucketId}/items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<IEnumerable<Item>> GetItems(int bucketId)
    {
        GetItems.Response response = await this.sender.Send(new GetItems.Query(bucketId));
        return this.mapper.Map<Item[]>(response.Items);
    }

    /// <summary>Retrieves an item from a bucket.</summary>
    /// <param name="bucketId">The Id. of the Bucket object to retrieve the item from.</param>
    /// <param name="itemId">The Id. of the Item object to be retrieved.</param>
    /// <returns>The specified Item object.</returns>
    /// <response code="200">Item retrieved successfully.</response>
    /// <response code="default">Problem Details (RFC 7807).</response>
    [HttpGet("{bucketId}/items/{itemId}", Name = "GetItem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<Item> GetItem(int bucketId, int itemId)
    {
        GetItem.Query query = new GetItem.Query(
            BucketId: bucketId,
            ItemId: itemId);

        GetItem.Response response = await this.sender.Send(query);

        return this.mapper.Map<Item>(response.Item);
    }

    /// <summary>Updates an item in a bucket or creates one if the specified one does not exist.</summary>
    /// <param name="bucketId">The Id. of the Bucket object to create or update the item in.</param>
    /// <param name="itemId">The Id. of the Item object to be created or updated.</param>
    /// <param name="requestBody">A complete set of key-value pairs to create or update the Item object with.</param>
    /// <returns>The created or updated Item object.</returns>
    /// <response code="200">Item updated successfully.</response>
    /// <response code="201">Item created successfully.</response>
    /// <response code="default">Problem Details (RFC 7807).</response>
    [HttpPut("{bucketId}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Item))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<Item>> UpdateItem(int bucketId, int itemId, [FromBody] UpdateItemRequestBody requestBody)
    {
        UpdateItem.Command command = new UpdateItem.Command(
            BucketId: bucketId,
            ItemId: itemId,
            Name: requestBody.Name,
            Description: requestBody.Description);

        UpdateItem.Response response = await this.sender.Send(command);
        Item item = this.mapper.Map<Item>(response.Item);

        return response.Created ? this.CreatedAtRoute("GetItem", new { bucketId, itemId }, item) : (ActionResult<Item>)item;
    }

    /// <summary>Deletes an item in a bucket.</summary>
    /// <param name="bucketId">The Id. of the Bucket object to delete the item from.</param>
    /// <param name="itemId">The Id. of the Item object to be deleted.</param>
    /// <returns>A "No Content (204)" HTTP status response.</returns>
    /// <response code="204">Item deleted successfully.</response>
    /// <response code="default">Problem Details (RFC 7807).</response>
    [HttpDelete("{bucketId}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RemoveItem(int bucketId, int itemId)
    {
        RemoveItem.Command command = new RemoveItem.Command(
            BucketId: bucketId,
            ItemId: itemId);

        await this.sender.Send(command);
        return this.NoContent();
    }
}
