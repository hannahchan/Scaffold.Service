namespace Scaffold.Application.UnitTests.Features.Item
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Features.Item;
    using Scaffold.Application.Repositories;
    using Scaffold.Data;
    using Scaffold.Domain.Entities;
    using Xunit;

    public class RemoveItemUnitTests
    {
        private readonly IBucketRepository repository;

        public RemoveItemUnitTests()
        {
            BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            this.repository = new BucketRepository(context);
        }

        public class Handler : RemoveItemUnitTests
        {
            [Fact]
            public async Task When_RemovingItemFromBucket_Expect_Removed()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item();
                bucket.AddItem(item);
                await this.repository.AddAsync(bucket);

                RemoveItem.Command command = new RemoveItem.Command { BucketId = bucket.Id, ItemId = item.Id };
                RemoveItem.Handler handler = new RemoveItem.Handler(this.repository);

                // Act
                await handler.Handle(command, default(CancellationToken));

                // Assert
                Assert.DoesNotContain(item, this.repository.Get(bucket.Id).Items);
            }

            [Fact]
            public async Task When_RemovingNonExistingItemFromBucket_Expect_NoChange()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item();
                bucket.AddItem(item);
                await this.repository.AddAsync(bucket);

                RemoveItem.Command command = new RemoveItem.Command
                {
                    BucketId = bucket.Id,
                    ItemId = new Random().Next(int.MaxValue)
                };

                RemoveItem.Handler handler = new RemoveItem.Handler(this.repository);

                // Act
                await handler.Handle(command, default(CancellationToken));

                // Assert
                Assert.NotEmpty(this.repository.Get(bucket.Id).Items);
            }

            [Fact]
            public async Task When_RemovingItemFromNonExistingBucket_Expect_NoChange()
            {
                // Arrange
                Bucket bucket = new Bucket();
                Item item = new Item();
                bucket.AddItem(item);
                await this.repository.AddAsync(bucket);

                RemoveItem.Command command = new RemoveItem.Command
                {
                    BucketId = new Random().Next(int.MaxValue),
                    ItemId = item.Id
                };

                RemoveItem.Handler handler = new RemoveItem.Handler(this.repository);

                // Act
                await handler.Handle(command, default(CancellationToken));

                // Assert
                Assert.NotEmpty(this.repository.Get(bucket.Id).Items);
            }
        }
    }
}
