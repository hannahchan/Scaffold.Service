namespace Scaffold.WebApi.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.WebApi.Controllers;
    using Scaffold.WebApi.Models.Bucket;
    using Scaffold.WebApi.Models.Item;
    using Xunit;

    public class BucketsControllerUnitTests
    {
        private readonly IMapper mapper;

        public BucketsControllerUnitTests()
        {
            MapperConfiguration configuration = new MapperConfiguration(config =>
            {
                config.AddProfile(new BucketMappingProfile());
                config.AddProfile(new ItemMappingProfile());
            });

            this.mapper = configuration.CreateMapper();
        }

        public class Buckets : BucketsControllerUnitTests
        {
            [Fact]
            public async Task When_AddingBucket_Expect_CreatedAtRouteResult()
            {
                // Arrange
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<AddBucket.Command>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new AddBucket.Response(new Domain.Aggregates.Bucket.Bucket()));

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

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
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<GetBuckets.Query>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new GetBuckets.Response(Array.Empty<Domain.Aggregates.Bucket.Bucket>()));

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

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
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<GetBucket.Query>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new GetBucket.Response(new Domain.Aggregates.Bucket.Bucket()));

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

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
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<UpdateBucket.Command>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new UpdateBucket.Response(new Domain.Aggregates.Bucket.Bucket(), false));

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

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
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<UpdateBucket.Command>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new UpdateBucket.Response(new Domain.Aggregates.Bucket.Bucket(), true));

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

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
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<RemoveBucket.Command>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(default(Unit));

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

                ActionResult result;

                // Act
                result = await controller.RemoveBucket(new Random().Next());

                // Assert
                Assert.IsType<NoContentResult>(result);
            }
        }

        public class Items : BucketsControllerUnitTests
        {
            [Fact]
            public async Task When_AddingItem_Expect_CreatedAtRouteResult()
            {
                // Arrange
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<AddItem.Command>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new AddItem.Response(new Domain.Aggregates.Bucket.Item()));

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

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
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<GetItems.Query>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new GetItems.Response(Array.Empty<Domain.Aggregates.Bucket.Item>()));

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

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
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<GetItem.Query>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new GetItem.Response(new Domain.Aggregates.Bucket.Item()));

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

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
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<UpdateItem.Command>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new UpdateItem.Response(new Domain.Aggregates.Bucket.Item(), false));

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

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
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<UpdateItem.Command>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new UpdateItem.Response(new Domain.Aggregates.Bucket.Item(), true));

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

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
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<RemoveItem.Command>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(default(Unit));

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

                ActionResult result;

                // Act
                result = await controller.RemoveItem(new Random().Next(), new Random().Next());

                // Assert
                Assert.IsType<NoContentResult>(result);
            }
        }
    }
}
