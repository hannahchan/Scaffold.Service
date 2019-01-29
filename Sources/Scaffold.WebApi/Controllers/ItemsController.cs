namespace Scaffold.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
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

        [HttpGet]
        public async Task<ActionResult<IList<Item>>> Get(int bucketId)
        {
            GetItems.Query query = new GetItems.Query { BucketId = bucketId };
            GetItems.Response response = await this.mediator.Send(query);

            return this.mapper.Map<List<Item>>(response.Items);
        }

        [HttpGet("{itemId}", Name = "GetItem")]
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

        [HttpPost]
        public async Task<ActionResult> Post(int bucketId, [FromBody] Item item)
        {
            AddItem.Command command = this.mapper.Map<AddItem.Command>(item);
            command.BucketId = bucketId;

            AddItem.Response response = await this.mediator.Send(command);
            item = this.mapper.Map<Item>(response.Item);

            return this.CreatedAtRoute("GetItem", new { itemId = item.Id }, item);
        }

        [HttpPatch("{itemId}")]
        public async Task<ActionResult<Item>> Patch(int bucketId, int itemId, [FromBody] Item item)
        {
            UpdateItem.Command command = this.mapper.Map<UpdateItem.Command>(item);
            command.BucketId = bucketId;
            command.ItemId = itemId;

            UpdateItem.Response response = await this.mediator.Send(command);

            return this.mapper.Map<Item>(response.Item);
        }

        [HttpDelete("{itemId}")]
        public async Task<ActionResult> Delete(int bucketId, int itemId)
        {
            await this.mediator.Send(new RemoveItem.Command { BucketId = bucketId, ItemId = itemId });
            return this.NoContent();
        }
    }
}
