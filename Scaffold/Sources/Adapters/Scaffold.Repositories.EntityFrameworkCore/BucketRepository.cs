namespace Scaffold.Repositories.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Interfaces;
    using Scaffold.Application.Models;
    using Scaffold.Domain.Aggregates.Bucket;

    public class BucketRepository : IBucketRepository
    {
        private readonly BucketContext context;

        public BucketRepository(BucketContext context)
        {
            this.context = context;
        }

        public void Add(Bucket bucket)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Set<Bucket>().Add(bucket);
            this.context.SaveChanges();
        }

        public Task AddAsync(Bucket bucket)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            return this.AddAsyncInternal(bucket);
        }

        public Bucket Get(int id)
        {
            return this.context.Set<Bucket>()
                .Where(bucket => bucket.Id == id)
                .Include(bucket => bucket.Items)
                .SingleOrDefault();
        }

        public List<Bucket> Get(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null, Ordering<Bucket> ordering = null)
        {
            return this.BuildQuery(predicate, limit, offset, ordering)
                .Include(bucket => bucket.Items)
                .ToList();
        }

        public async Task<Bucket> GetAsync(int id)
        {
            return await this.context.Set<Bucket>()
                .Where(bucket => bucket.Id == id)
                .Include(bucket => bucket.Items)
                .SingleOrDefaultAsync();
        }

        public Task<List<Bucket>> GetAsync(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null, Ordering<Bucket> ordering = null)
        {
            return this.BuildQuery(predicate, limit, offset, ordering)
                .Include(bucket => bucket.Items)
                .ToListAsync();
        }

        public void Remove(Bucket bucket)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Set<Bucket>().Remove(bucket);
            this.context.SaveChanges();
        }

        public Task RemoveAsync(Bucket bucket)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            return this.RemoveAsyncInternal(bucket);
        }

        public void Update(Bucket bucket)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Set<Bucket>().Update(bucket);
            this.context.SaveChanges();
        }

        public Task UpdateAsync(Bucket bucket)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            return this.UpdateAsyncInternal(bucket);
        }

        private async Task AddAsyncInternal(Bucket bucket)
        {
            this.context.Set<Bucket>().Add(bucket);
            await this.context.SaveChangesAsync();
        }

        private IQueryable<Bucket> BuildQuery(Expression<Func<Bucket, bool>> predicate, int? limit, int? offset, Ordering<Bucket> ordering = null)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            IQueryable<Bucket> query = this.context.Set<Bucket>().Where(predicate);

            if (ordering != null)
            {
                foreach (OrderBy orderBy in ordering)
                {
                    string methodName = orderBy.Ascending ? nameof(Queryable.ThenBy) : nameof(Queryable.ThenByDescending);

                    if (orderBy == ordering.First())
                    {
                        methodName = orderBy.Ascending ? nameof(Queryable.OrderBy) : nameof(Queryable.OrderByDescending);
                    }

                    ParameterExpression parameterExpression = Expression.Parameter(typeof(Bucket));
                    MemberExpression memberExpression = Expression.Property(parameterExpression, typeof(Bucket).GetProperty(orderBy.PropertyName));
                    LambdaExpression lambdaExpression = Expression.Lambda(memberExpression, new ParameterExpression[] { parameterExpression });

                    query = query.Provider.CreateQuery<Bucket>(Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new Type[] { typeof(Bucket), lambdaExpression.ReturnType },
                        query.Expression,
                        Expression.Quote(lambdaExpression)));
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

        private async Task RemoveAsyncInternal(Bucket bucket)
        {
            this.context.Set<Bucket>().Remove(bucket);
            await this.context.SaveChangesAsync();
        }

        private async Task UpdateAsyncInternal(Bucket bucket)
        {
            this.context.Set<Bucket>().Update(bucket);
            await this.context.SaveChangesAsync();
        }
    }
}
