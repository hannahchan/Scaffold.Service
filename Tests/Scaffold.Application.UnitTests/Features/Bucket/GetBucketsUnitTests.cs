namespace Scaffold.Application.UnitTests.Features.Bucket
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.Application.Repositories;
    using Scaffold.Data;
    using Scaffold.Domain.Entities;
    using Xunit;

    public class GetBucketsUnitTests
    {
        private readonly IBucketRepository repository;

        public GetBucketsUnitTests()
        {
            BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            this.repository = new BucketRepository(context);
        }

        public class Handler : GetBucketsUnitTests
        {
            [Fact]
            public async Task When_GettingBuckets_Expect_ExistingBuckets()
            {
                // Arrange
                await this.repository.AddAsync(new Bucket());
                await this.repository.AddAsync(new Bucket());
                await this.repository.AddAsync(new Bucket());

                GetBuckets.Query query = new GetBuckets.Query();
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default(CancellationToken));

                // Assert
                Assert.NotNull(response.Buckets);
                Assert.NotEmpty(response.Buckets);
                Assert.Equal(3, response.Buckets.Count);
            }

            [Fact]
            public async Task When_GettingNonExistingBuckets_Expect_EmptyList()
            {
                // Arrange
                GetBuckets.Query query = new GetBuckets.Query();
                GetBuckets.Handler handler = new GetBuckets.Handler(this.repository);

                // Act
                GetBuckets.Response response = await handler.Handle(query, default(CancellationToken));

                // Assert
                Assert.NotNull(response.Buckets);
                Assert.Empty(response.Buckets);
            }
        }
    }
}
