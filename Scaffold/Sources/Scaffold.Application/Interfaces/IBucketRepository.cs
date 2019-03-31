namespace Scaffold.Application.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Scaffold.Domain.Entities;

    public interface IBucketRepository
    {
        void Add(Bucket bucket);

        Task AddAsync(Bucket bucket);

        Bucket Get(int id);

        IList<Bucket> Get(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null);

        Task<Bucket> GetAsync(int id);

        Task<IList<Bucket>> GetAsync(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null);

        void Remove(Bucket bucket);

        Task RemoveAsync(Bucket bucket);

        void Update(Bucket bucket);

        Task UpdateAsync(Bucket bucket);
    }
}
