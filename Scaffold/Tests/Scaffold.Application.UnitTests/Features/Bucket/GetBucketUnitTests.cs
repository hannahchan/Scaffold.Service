namespace Scaffold.Application.UnitTests.Features.Bucket
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.Application.Interfaces;
    using Scaffold.Data;
    using Scaffold.Data.Repositories;
    using Scaffold.Domain.Entities;
    using Xunit;

    public class GetBucketUnitTests
    {
        private readonly IBucketRepository repository;

        public GetBucketUnitTests()
        {
            BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            this.repository = new BucketRepository(context);
        }

        public class Handler : GetBucketUnitTests
        {
            [Fact]
            public async Task When_GettingBucket_Expect_ExistingBucket()
            {
                // Arrange
                Bucket bucket = new Bucket();
                await this.repository.AddAsync(bucket);

                GetBucket.Query query = new GetBucket.Query { Id = bucket.Id };
                GetBucket.Handler handler = new GetBucket.Handler(this.repository);

                // Act
                GetBucket.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Equal(bucket.Id, response.Bucket.Id);
                Assert.Equal(bucket.Name, response.Bucket.Name);
            }

            [Fact]
            public async Task When_GettingNonExistingBucket_Expect_Null()
            {
                // Arrange
                GetBucket.Query query = new GetBucket.Query { Id = new Random().Next(int.MaxValue) };
                GetBucket.Handler handler = new GetBucket.Handler(this.repository);

                // Act
                GetBucket.Response response = await handler.Handle(query, default);

                // Assert
                Assert.Null(response.Bucket);
            }
        }
    }
}
