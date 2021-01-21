namespace Scaffold.Application.UnitTests.Features.Bucket
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.Application.Interfaces;
    using Scaffold.Domain.Aggregates.Bucket;
    using Scaffold.Repositories;
    using Xunit;

    public class GetItemUnitTests
    {
        private readonly IBucketRepository repository;

        public GetItemUnitTests()
        {
            BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            this.repository = new BucketRepository(context);
        }

        public class Response
        {
            [Fact]
            public void When_InstantiatingResponseWithItem_Expect_ResponseWithItem()
            {
                // Arrange
                Item item = new Item();

                // Act
                GetItem.Response response = new GetItem.Response(item);

                // Assert
                Assert.Equal(item, response.Item);
            }

            [Fact]
            public void When_InstantiatingResponseWithNull_Expect_ArgumentNullException()
            {
                // Act
                Exception exception = Record.Exception(() => new GetItem.Response(null));

                // Assert
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("item", argumentNullException.ParamName);
            }
        }

        public class Handler : GetItemUnitTests
        {
            [Fact]
            public async Task When_GettingItemFromBucket_Expect_ExistingItem()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item();
                bucket.AddItem(item);
                await this.repository.AddAsync(bucket);

                GetItem.Query query = new GetItem.Query(
                    bucketId: bucket.Id,
                    itemId: item.Id);

                GetItem.Handler handler = new GetItem.Handler(this.repository);

                // Act
                GetItem.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Equal(item.Id, response.Item.Id);
                Assert.Equal(item.Name, response.Item.Name);
            }

            [Fact]
            public async Task When_GettingNonExistingItemFromBucket_Expect_ItemNotFoundException()
            {
                // Arrange
                Bucket bucket = new Bucket();
                await this.repository.AddAsync(bucket);

                GetItem.Query query = new GetItem.Query(
                    bucketId: bucket.Id,
                    itemId: new Random().Next());

                GetItem.Handler handler = new GetItem.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(query, default));

                // Assert
                Assert.IsType<ItemNotFoundException>(exception);
            }

            [Fact]
            public async Task When_GettingItemFromNonExistingBucket_Expect_BucketNotFoundException()
            {
                // Arrange
                GetItem.Query query = new GetItem.Query(
                    bucketId: new Random().Next(),
                    itemId: new Random().Next());

                GetItem.Handler handler = new GetItem.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(query, default));

                // Assert
                Assert.IsType<BucketNotFoundException>(exception);
            }
        }
    }
}
