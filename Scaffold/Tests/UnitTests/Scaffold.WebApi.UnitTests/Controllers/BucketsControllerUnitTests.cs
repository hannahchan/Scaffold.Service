namespace Scaffold.WebApi.UnitTests.Controllers;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Scaffold.Application.Components.Bucket;
using Scaffold.WebApi.Controllers;
using Scaffold.WebApi.Models.Bucket;
using Scaffold.WebApi.Models.Item;
using Xunit;

public class BucketsControllerUnitTests
{
    private readonly IMapper mapper;

    private readonly Mock.Sender sender;

    public BucketsControllerUnitTests()
    {
        MapperConfiguration configuration = new MapperConfiguration(config =>
        {
            config.AddProfile(new BucketMappingProfile());
            config.AddProfile(new ItemMappingProfile());
        });

        this.mapper = configuration.CreateMapper();
        this.sender = new Mock.Sender();
    }

    public class Buckets : BucketsControllerUnitTests
    {
        [Fact]
        public async Task When_AddingBucket_Expect_CreatedAtRouteResult()
        {
            // Arrange
            this.sender.SetResponse(new AddBucket.Response(new Domain.Aggregates.Bucket.Bucket()));
            BucketsController controller = new BucketsController(this.mapper, this.sender);

            ActionResult result;

            // Act
            result = await controller.AddBucket(new AddBucketRequestBody());

            // Assert
            CreatedAtRouteResult actionResult = Assert.IsType<CreatedAtRouteResult>(result);

            Assert.NotEmpty(actionResult.RouteName);
            Assert.IsType<Bucket>(actionResult.Value);
        }

        [Fact]
        public async Task When_GettingBuckets_Expect_Buckets()
        {
            // Arrange
            this.sender.SetResponse(new GetBuckets.Response(Array.Empty<Domain.Aggregates.Bucket.Bucket>()));
            BucketsController controller = new BucketsController(this.mapper, this.sender);

            IEnumerable<Bucket> result;

            // Act
            result = await controller.GetBuckets(new GetBucketsRequestQuery());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task When_GettingBucket_Expect_Bucket()
        {
            // Arrange
            this.sender.SetResponse(new GetBucket.Response(new Domain.Aggregates.Bucket.Bucket()));
            BucketsController controller = new BucketsController(this.mapper, this.sender);

            Bucket result;

            // Act
            result = await controller.GetBucket(new Random().Next());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task When_UpdatingBucket_Expect_UpdatedBucket()
        {
            // Arrange
            this.sender.SetResponse(new UpdateBucket.Response(new Domain.Aggregates.Bucket.Bucket(), false));
            BucketsController controller = new BucketsController(this.mapper, this.sender);

            ActionResult<Bucket> result;

            // Act
            result = await controller.UpdateBucket(new Random().Next(), new UpdateBucketRequestBody());

            // Assert
            Assert.Null(result.Result);
            Assert.IsType<Bucket>(result.Value);
        }

        [Fact]
        public async Task When_UpdatingNonExistingBucket_Expect_CreatedAtRouteResult()
        {
            // Arrange
            this.sender.SetResponse(new UpdateBucket.Response(new Domain.Aggregates.Bucket.Bucket(), true));
            BucketsController controller = new BucketsController(this.mapper, this.sender);

            ActionResult<Bucket> result;

            // Act
            result = await controller.UpdateBucket(new Random().Next(), new UpdateBucketRequestBody());

            // Assert
            CreatedAtRouteResult actionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
            Assert.Null(result.Value);

            Assert.NotEmpty(actionResult.RouteName);
            Assert.IsType<Bucket>(actionResult.Value);
        }

        [Fact]
        public async Task When_RemovingBucket_Expect_NoContentResult()
        {
            // Arrange
            this.sender.SetResponse(default(Unit));
            BucketsController controller = new BucketsController(this.mapper, this.sender);

            ActionResult result;

            // Act
            result = await controller.RemoveBucket(new Random().Next());

            // Assert
            Assert.IsType<NoContentResult>(result);

            Assert.Collection(
                this.sender.ReceivedRequests,
                receivedRequest => Assert.IsType<RemoveBucket.Command>(receivedRequest.Request));
        }
    }

    public class Items : BucketsControllerUnitTests
    {
        [Fact]
        public async Task When_AddingItem_Expect_CreatedAtRouteResult()
        {
            // Arrange
            this.sender.SetResponse(new AddItem.Response(new Domain.Aggregates.Bucket.Item()));
            BucketsController controller = new BucketsController(this.mapper, this.sender);

            ActionResult result;

            // Act
            result = await controller.AddItem(new Random().Next(), new AddItemRequestBody());

            // Assert
            CreatedAtRouteResult actionResult = Assert.IsType<CreatedAtRouteResult>(result);

            Assert.NotEmpty(actionResult.RouteName);
            Assert.IsType<Item>(actionResult.Value);
        }

        [Fact]
        public async Task When_GettingItems_Expect_Items()
        {
            // Arrange
            this.sender.SetResponse(new GetItems.Response(Array.Empty<Domain.Aggregates.Bucket.Item>()));
            BucketsController controller = new BucketsController(this.mapper, this.sender);

            IEnumerable<Item> result;

            // Act
            result = await controller.GetItems(new Random().Next());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task When_GettingItem_Expect_Item()
        {
            // Arrange
            this.sender.SetResponse(new GetItem.Response(new Domain.Aggregates.Bucket.Item()));
            BucketsController controller = new BucketsController(this.mapper, this.sender);

            Item result;

            // Act
            result = await controller.GetItem(new Random().Next(), new Random().Next());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task When_UpdatingItem_Expect_UpdatedItem()
        {
            // Arrange
            this.sender.SetResponse(new UpdateItem.Response(new Domain.Aggregates.Bucket.Item(), false));
            BucketsController controller = new BucketsController(this.mapper, this.sender);

            ActionResult<Item> result;

            // Act
            result = await controller.UpdateItem(new Random().Next(), new Random().Next(), new UpdateItemRequestBody());

            // Assert
            Assert.Null(result.Result);
            Assert.IsType<Item>(result.Value);
        }

        [Fact]
        public async Task When_UpdatingNonExistingItem_Expect_CreatedAtRouteResult()
        {
            // Arrange
            this.sender.SetResponse(new UpdateItem.Response(new Domain.Aggregates.Bucket.Item(), true));
            BucketsController controller = new BucketsController(this.mapper, this.sender);

            ActionResult<Item> result;

            // Act
            result = await controller.UpdateItem(new Random().Next(), new Random().Next(), new UpdateItemRequestBody());

            // Assert
            CreatedAtRouteResult actionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
            Assert.Null(result.Value);

            Assert.NotEmpty(actionResult.RouteName);
            Assert.IsType<Item>(actionResult.Value);
        }

        [Fact]
        public async Task When_RemovingItem_Expect_NoContentResult()
        {
            // Arrange
            this.sender.SetResponse(default(Unit));
            BucketsController controller = new BucketsController(this.mapper, this.sender);

            ActionResult result;

            // Act
            result = await controller.RemoveItem(new Random().Next(), new Random().Next());

            // Assert
            Assert.IsType<NoContentResult>(result);

            Assert.Collection(
                this.sender.ReceivedRequests,
                receivedRequest => Assert.IsType<RemoveItem.Command>(receivedRequest.Request));
        }
    }
}
