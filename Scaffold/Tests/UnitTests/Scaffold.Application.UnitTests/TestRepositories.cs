#pragma warning disable IDISP004 // Don't ignore created IDisposable

namespace Scaffold.Application.UnitTests;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Application.Components.Bucket;
using Scaffold.Repositories;
using Xunit;

public sealed class TestRepositories : TheoryData<IBucketRepository>, IDisposable
{
    private readonly ServiceProvider serviceProvider = CreateServiceProvider();

    private bool disposed;

    public TestRepositories()
    {
        this.Add(new ScopedBucketRepository(this.CreateNewBucketContext()));
    }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        this.serviceProvider?.Dispose();
    }

    private static ServiceProvider CreateServiceProvider()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDbContextFactory<BucketContext>(builder => builder.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        return services.BuildServiceProvider();
    }

    private BucketContext CreateNewBucketContext()
    {
        return this.serviceProvider
            .GetRequiredService<IDbContextFactory<BucketContext>>()
            .CreateDbContext();
    }
}
