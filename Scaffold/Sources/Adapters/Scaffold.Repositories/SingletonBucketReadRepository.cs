namespace Scaffold.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scaffold.Application.Common.Interfaces;
using Scaffold.Application.Common.Models;
using Scaffold.Domain.Aggregates.Bucket;

public class SingletonBucketReadRepository : IBucketReadRepository
{
    private readonly IDbContextFactory<BucketContext> factory;

    public SingletonBucketReadRepository(IDbContextFactory<BucketContext.ReadOnly> factory)
    {
        this.factory = factory as IDbContextFactory<BucketContext>;
    }

    protected SingletonBucketReadRepository(IDbContextFactory<BucketContext> factory)
    {
        this.factory = factory;
    }

    public Bucket? Get(int id)
    {
        using BucketContext context = this.factory.CreateDbContext();

        return BuildQuery(context, bucket => bucket.Id == id)
            .Include(bucket => bucket.Items)
            .SingleOrDefault();
    }

    public List<Bucket> Get(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null, SortOrder<Bucket>? sortOrder = null)
    {
        using BucketContext context = this.factory.CreateDbContext();

        return BuildQuery(context, predicate, limit, offset, sortOrder)
            .Include(bucket => bucket.Items)
            .ToList();
    }

    public Task<Bucket?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        using BucketContext context = this.factory.CreateDbContext();

        return BuildQuery(context, bucket => bucket.Id == id)
            .Include(bucket => bucket.Items)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<List<Bucket>> GetAsync(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null, SortOrder<Bucket>? sortOrder = null, CancellationToken cancellationToken = default)
    {
        using BucketContext context = this.factory.CreateDbContext();

        return BuildQuery(context, predicate, limit, offset, sortOrder)
            .Include(bucket => bucket.Items)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<Bucket> BuildQuery(BucketContext context, Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null, SortOrder<Bucket>? sortOrder = null)
    {
        IQueryable<Bucket> query = context.Buckets.Where(predicate);

        if (sortOrder != null)
        {
            foreach ((LambdaExpression KeySelector, bool Descending) sortItem in sortOrder)
            {
                string methodName = sortItem.Descending ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy);

                if (sortItem == sortOrder[0])
                {
                    methodName = sortItem.Descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
                }

                query = query.Provider.CreateQuery<Bucket>(Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { typeof(Bucket), sortItem.KeySelector.ReturnType },
                    query.Expression,
                    sortItem.KeySelector));
            }
        }

        if (offset != null)
        {
            query = query.Skip(offset.GetValueOrDefault());
        }

        if (limit != null)
        {
            query = query.Take(limit.GetValueOrDefault());
        }

        return query;
    }
}
