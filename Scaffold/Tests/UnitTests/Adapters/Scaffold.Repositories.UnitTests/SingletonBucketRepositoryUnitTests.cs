namespace Scaffold.Repositories.UnitTests;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class SingletonBucketRepositoryUnitTests
{
    private readonly IDbContextFactory<BucketContext> bucketContextFactory;

    public SingletonBucketRepositoryUnitTests()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDbContextFactory<BucketContext>(builder => builder.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        #pragma warning disable IDISP001 // Dispose created
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        #pragma warning restore IDISP001 // Dispose created

        this.bucketContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
    }

    public class AddBucket : SingletonBucketRepositoryUnitTests
    {
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
