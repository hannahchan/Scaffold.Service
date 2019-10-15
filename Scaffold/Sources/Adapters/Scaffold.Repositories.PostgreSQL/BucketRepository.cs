namespace Scaffold.Repositories.PostgreSQL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Scaffold.Application.Interfaces;
    using Scaffold.Application.Models;
    using Scaffold.Domain.Aggregates.Bucket;

    public class BucketRepository : IBucketRepository
    {
        private static readonly Dictionary<string, LambdaExpression> KeySelectors = InitializeKeySelectors();

        private readonly BucketContext context;

        public BucketRepository(BucketContext context)
        {
            this.context = context;
        }

        public void Add(Bucket bucket)
        {
            if (bucket is null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Set<Bucket>().Add(bucket);
            this.context.SaveChanges();
        }

        public Task AddAsync(Bucket bucket)
        {
            if (bucket is null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Set<Bucket>().Add(bucket);
            return this.context.SaveChangesAsync();
        }

        public Bucket? Get(int id)
        {
            return this.context.Set<Bucket>()
                .Where(bucket => bucket.Id == id)
                .Include(bucket => bucket.Items)
                .SingleOrDefault();
        }

        public List<Bucket> Get(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null, Ordering<Bucket>? ordering = null)
        {
            return this.BuildQuery(predicate, limit, offset, ordering)
                .Include(bucket => bucket.Items)
                .ToList();
        }

        public Task<Bucket?> GetAsync(int id)
        {
            return this.context.Set<Bucket>()
                .Where(bucket => bucket.Id == id)
                .Include(bucket => bucket.Items)
                .SingleOrDefaultAsync();
        }

        public Task<List<Bucket>> GetAsync(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null, Ordering<Bucket>? ordering = null)
        {
            return this.BuildQuery(predicate, limit, offset, ordering)
                .Include(bucket => bucket.Items)
                .ToListAsync();
        }

        public void Remove(Bucket bucket)
        {
            if (bucket is null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Set<Bucket>().Remove(bucket);
            this.context.SaveChanges();
        }

        public Task RemoveAsync(Bucket bucket)
        {
            if (bucket is null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Set<Bucket>().Remove(bucket);
            return this.context.SaveChangesAsync();
        }

        public void Update(Bucket bucket)
        {
            if (bucket is null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Set<Bucket>().Update(bucket);
            this.context.SaveChanges();
        }

        public Task UpdateAsync(Bucket bucket)
        {
            if (bucket is null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }

            this.context.Set<Bucket>().Update(bucket);
            return this.context.SaveChangesAsync();
        }

        private static Dictionary<string, LambdaExpression> InitializeKeySelectors()
        {
            Dictionary<string, LambdaExpression> keySelectors = new Dictionary<string, LambdaExpression>();

            ParameterExpression parameterExpression = Expression.Parameter(typeof(Bucket));

            foreach (PropertyInfo property in typeof(Bucket).GetProperties())
            {
                MemberExpression memberExpression = Expression.Property(parameterExpression, typeof(Bucket).GetProperty(property.Name));
                LambdaExpression lambdaExpression = Expression.Lambda(memberExpression, new ParameterExpression[] { parameterExpression });

                keySelectors.Add(property.Name, lambdaExpression);
            }

            return keySelectors;
        }

        private IQueryable<Bucket> BuildQuery(Expression<Func<Bucket, bool>> predicate, int? limit, int? offset, Ordering<Bucket>? ordering = null)
        {
            if (predicate is null)
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

                    query = query.Provider.CreateQuery<Bucket>(Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new Type[] { typeof(Bucket), KeySelectors[orderBy.PropertyName].ReturnType },
                        query.Expression,
                        Expression.Quote(KeySelectors[orderBy.PropertyName])));
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
}
