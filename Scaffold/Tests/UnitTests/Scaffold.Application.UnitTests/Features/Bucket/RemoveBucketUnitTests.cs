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

    public class RemoveBucketUnitTests
    {
        private readonly IBucketRepository repository;

        public RemoveBucketUnitTests()
        {
            BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            this.repository = new BucketRepository(context);
        }

        public class Handler : RemoveBucketUnitTests
        {
            [Fact]
            public async Task When_RemovingBucket_Expect_Removed()
            {
                // Arrange
                Bucket bucket = new Bucket();
                await this.repository.AddAsync(bucket);

                RemoveBucket.Command command = new RemoveBucket.Command(bucket.Id);
                RemoveBucket.Handler handler = new RemoveBucket.Handler(this.repository);

                // Act
                await handler.Handle(command, default);

                // Assert
                Assert.Null(this.repository.Get(bucket.Id));
            }

            [Fact]
            public async Task When_RemovingNonExistingBucket_Expect_BucketNotFoundException()
            {
                // Arrange
                RemoveBucket.Command command = new RemoveBucket.Command(new Random().Next());
                RemoveBucket.Handler handler = new RemoveBucket.Handler(this.repository);

                // Act
                Exception exception = await Record.ExceptionAsync(() => handler.Handle(command, default));

                // Assert
                Assert.IsType<BucketNotFoundException>(exception);
            }
        }
    }
}
