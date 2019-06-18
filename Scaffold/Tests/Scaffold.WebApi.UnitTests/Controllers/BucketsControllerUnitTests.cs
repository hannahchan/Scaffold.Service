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
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.WebApi.Controllers;
    using Scaffold.WebApi.Views;
    using Scaffold.WebApi.Views.MappingProfiles;
    using Xunit;

    public class BucketsControllerUnitTests
    {
        private readonly IMapper mapper;

        public BucketsControllerUnitTests()
        {
            BucketMappingProfile profile = new BucketMappingProfile();
            MapperConfiguration configuration = new MapperConfiguration(config => config.AddProfile(profile));
            this.mapper = configuration.CreateMapper();
        }

        public class Post : BucketsControllerUnitTests
        {
            [Fact]
            public async Task When_CreatingBucket_Expect_CreatedAtRouteResult()
            {
                // Arrange
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<AddBucket.Command>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new AddBucket.Response { Bucket = new Domain.Entities.Bucket() });

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

                ActionResult result;

                // Act
                result = await controller.Post(new Bucket());

                // Assert
                Assert.IsType<CreatedAtRouteResult>(result);

                CreatedAtRouteResult actionResult = result as CreatedAtRouteResult;

                Assert.NotEmpty(actionResult.RouteName);
                Assert.IsType<Bucket>(actionResult.Value);
            }
        }

        public class Get : BucketsControllerUnitTests
        {
            [Fact]
            public async Task When_GettingBuckets_Expect_Buckets()
            {
                // Arrange
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<GetBuckets.Query>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new GetBuckets.Response { Buckets = new List<Domain.Entities.Bucket>() });

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

                IList<Bucket> result;

                // Act
                result = await controller.Get(null, null);

                // Assert
                Assert.NotNull(result);
            }

            [Fact]
            public async Task When_GettingBucket_Expect_Bucket()
            {
                // Arrange
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<GetBucket.Query>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new GetBucket.Response { Bucket = new Domain.Entities.Bucket() });

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

                Bucket result;

                // Act
                result = await controller.Get(new Random().Next(int.MaxValue));

                // Assert
                Assert.NotNull(result);
            }

            [Fact]
            public async Task When_GettingNonExistingBucket_Expect_BucketNotFoundException()
            {
                // Arrange
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<GetBucket.Query>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new GetBucket.Response());

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

                Exception exception;

                // Act
                exception = await Record.ExceptionAsync(() => controller.Get(new Random().Next(int.MaxValue)));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<BucketNotFoundException>(exception);
            }
        }

        public class Put : BucketsControllerUnitTests
        {
            [Fact]
            public async Task When_UpdatingBucket_Expect_UpdatedBucket()
            {
                // Arrange
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<UpdateBucket.Command>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new UpdateBucket.Response
                    {
                        Bucket = new Domain.Entities.Bucket(),
                        Created = false,
                    });

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

                ActionResult<Bucket> result;

                // Act
                result = await controller.Put(new Random().Next(int.MaxValue), new Bucket());

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
                    .ReturnsAsync(new UpdateBucket.Response
                    {
                        Bucket = new Domain.Entities.Bucket(),
                        Created = true,
                    });

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

                ActionResult<Bucket> result;

                // Act
                result = await controller.Put(new Random().Next(int.MaxValue), new Bucket());

                // Assert
                Assert.IsType<CreatedAtRouteResult>(result.Result);
                Assert.Null(result.Value);

                CreatedAtRouteResult actionResult = result.Result as CreatedAtRouteResult;

                Assert.NotEmpty(actionResult.RouteName);
                Assert.IsType<Bucket>(actionResult.Value);
            }
        }

        public class Delete : BucketsControllerUnitTests
        {
            [Fact]
            public async Task When_DeletingBucket_Expect_NoContentResult()
            {
                // Arrange
                Mock<IMediator> mock = new Mock<IMediator>();
                mock.Setup(m => m.Send(It.IsAny<RemoveBucket.Command>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(default(Unit));

                BucketsController controller = new BucketsController(this.mapper, mock.Object);

                ActionResult result;

                // Act
                result = await controller.Delete(new Random().Next(int.MaxValue));

                // Assert
                Assert.IsType<NoContentResult>(result);
            }
        }
    }
}
