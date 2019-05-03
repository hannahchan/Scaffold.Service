namespace Scaffold.Application.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Scaffold.Application.Models;
    using Scaffold.Domain.Entities;

    public interface IBucketReadRepository
    {
        Bucket Get(int id);

        IList<Bucket> Get(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null, Ordering<Bucket> ordering = null);

        Task<Bucket> GetAsync(int id);

        Task<IList<Bucket>> GetAsync(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null, Ordering<Bucket> ordering = null);
    }
}
