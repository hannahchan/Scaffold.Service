namespace Scaffold.Repositories.UnitTests;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Domain.Aggregates.Bucket;
using Xunit;

public class SingletonBucketRepositoryUnitTests
{
    private static ServiceProvider CreateNewServiceProvider()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDbContextFactory<BucketContext>(builder => builder.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        return services.BuildServiceProvider();
    }

    public class AddBucket : SingletonBucketRepositoryUnitTests
    {
        [Fact]
        public void When_AddingBucket_Expect_Saved()
        {
            // Arrange
            using ServiceProvider serviceProvider = CreateNewServiceProvider();

            IDbContextFactory<BucketContext> contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
            SingletonBucketRepository repository = new SingletonBucketRepository(contextFactory);

            Bucket bucket = new Bucket();

            // Act
            repository.Add(bucket);

            // Assert
            using BucketContext context = contextFactory.CreateDbContext();

            Assert.Single(context.Buckets);

            Bucket result = context.Buckets.Find(bucket.Id);
            Assert.NotEqual(bucket, result);
            Assert.Equal(bucket.Id, result.Id);
        }
    }

    public class AddBuckets : SingletonBucketRepositoryUnitTests
    {
    }

    public class AddBucketAsync : SingletonBucketRepositoryUnitTests
    {
    }

    public class AddBucketsAsync : SingletonBucketRepositoryUnitTests
    {
    }

    public class RemoveBucket : SingletonBucketRepositoryUnitTests
    {
    }

    public class RemoveBuckets : SingletonBucketRepositoryUnitTests
    {
    }

    public class RemoveBucketAsync : SingletonBucketRepositoryUnitTests
    {
    }

    public class RemoveBucketsAsync : SingletonBucketRepositoryUnitTests
    {
    }

    public class UpdateBucket : SingletonBucketRepositoryUnitTests
    {
    }

    public class UpdateBuckets : SingletonBucketRepositoryUnitTests
    {
    }

    public class UpdateBucketAsync : SingletonBucketRepositoryUnitTests
    {
    }

    public class UpdateBucketsAsync : SingletonBucketRepositoryUnitTests
    {
    }
}
