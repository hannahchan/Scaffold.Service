#pragma warning disable IDISP004 // Don't ignore created IDisposable

namespace Scaffold.Application.UnitTests;

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Application.Components.Bucket;
using Scaffold.Repositories;
using Xunit;

public sealed class TestRepositories : TheoryData<IBucketRepository>, IDisposable
{
    private readonly List<IDisposable> disposables = new List<IDisposable>();

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

        foreach (IDisposable disposable in this.disposables)
        {
            disposable?.Dispose();
        }
    }

    private BucketContext CreateNewBucketContext()
    {
        BucketContext context = new BucketContext(new DbContextOptionsBuilder<BucketContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        this.disposables.Add(context);

        return context;
    }

    private IDbContextFactory<BucketContext> CreateNewBucketContextFactory()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDbContextFactory<BucketContext>(builder => builder.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        this.disposables.Add(serviceProvider);

        return serviceProvider.GetRequiredService<IDbContextFactory<BucketContext>>();
    }
}
