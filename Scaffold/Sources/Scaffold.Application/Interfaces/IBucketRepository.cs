namespace Scaffold.Application.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using Scaffold.Domain.Aggregates.Bucket;

    public interface IBucketRepository : IBucketReadRepository
    {
        void Add(Bucket bucket);

        Task AddAsync(Bucket bucket, CancellationToken cancellationToken = default);

        void Remove(Bucket bucket);

        Task RemoveAsync(Bucket bucket, CancellationToken cancellationToken = default);

        void Update(Bucket bucket);

        Task UpdateAsync(Bucket bucket, CancellationToken cancellationToken = default);
    }
}
