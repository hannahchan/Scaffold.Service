namespace Scaffold.Application.UnitTests.Features.Item
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Features.Item;
    using Scaffold.Application.Interfaces;
    using Scaffold.Application.Repositories;
    using Scaffold.Data;
    using Scaffold.Domain.Entities;
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

                GetItem.Query query = new GetItem.Query { BucketId = bucket.Id, ItemId = item.Id };
                GetItem.Handler handler = new GetItem.Handler(this.repository);

                // Act
                GetItem.Response response = await handler.Handle(query, default(CancellationToken));

                // Assert
                Assert.Equal(item.Id, response.Item.Id);
                Assert.Equal(item.Name, response.Item.Name);
            }

            [Fact]
            public async Task When_GettingNonExistingItemFromBucket_Expect_Null()
            {
                // Arrange
                Bucket bucket = new Bucket();
                await this.repository.AddAsync(bucket);

                GetItem.Query query = new GetItem.Query
                {
                    BucketId = bucket.Id,
                    ItemId = new Random().Next(int.MaxValue)
                };

                GetItem.Handler handler = new GetItem.Handler(this.repository);

                // Act
                GetItem.Response response = await handler.Handle(query, default(CancellationToken));

                // Assert
                Assert.Null(response.Item);
            }

            [Fact]
            public async void When_GettingItemFromNonExistingBucket_Expect_BucketNotFoundException()
            {
                // Arrange
                GetItem.Query query = new GetItem.Query
                {
                    BucketId = new Random().Next(int.MaxValue),
                    ItemId = new Random().Next(int.MaxValue)
                };

                GetItem.Handler handler = new GetItem.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() =>
                    handler.Handle(query, default(CancellationToken)));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<BucketNotFoundException>(exception);
            }
        }
    }
}
