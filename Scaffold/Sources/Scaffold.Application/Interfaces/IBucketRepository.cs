namespace Scaffold.Application.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Scaffold.Domain.Entities;

    public interface IBucketRepository : IBucketReadRepository
    {
        void Add(Bucket bucket);

        Task AddAsync(Bucket bucket);

        void Remove(Bucket bucket);

        Task RemoveAsync(Bucket bucket);

        void Update(Bucket bucket);

        Task UpdateAsync(Bucket bucket);
    }
}
