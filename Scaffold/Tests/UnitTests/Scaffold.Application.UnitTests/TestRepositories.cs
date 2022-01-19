namespace Scaffold.Application.UnitTests;

using System;
using Microsoft.EntityFrameworkCore;
using Scaffold.Application.Components.Bucket;
using Scaffold.Repositories;
using Xunit;

public class TestRepositories : TheoryData<IBucketRepository>
{
    public TestRepositories()
    {
        this.Add(new ScopedBucketRepository(CreateNewBucketContext()));
    }

    private static BucketContext CreateNewBucketContext()
    {
        return new BucketContext(new DbContextOptionsBuilder<BucketContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
    }
}
