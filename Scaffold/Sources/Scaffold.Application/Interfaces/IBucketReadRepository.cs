namespace Scaffold.Application.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Scaffold.Application.Models;
    using Scaffold.Domain.Aggregates.Bucket;

    public interface IBucketReadRepository
    {
        Bucket Get(int id);

        List<Bucket> Get(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null, Ordering<Bucket>? ordering = null);

        Task<Bucket> GetAsync(int id);

        Task<List<Bucket>> GetAsync(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null, Ordering<Bucket>? ordering = null);
    }
}
