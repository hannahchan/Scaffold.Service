namespace Scaffold.Application.Components.Bucket;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scaffold.Application.Common.Interfaces;
using Scaffold.Domain.Aggregates.Bucket;

public interface IBucketRepository : IBucketReadRepository
{
    void Add(Bucket bucket);

    void Add(IEnumerable<Bucket> buckets);

    Task AddAsync(Bucket bucket, CancellationToken cancellationToken = default);

    Task AddAsync(IEnumerable<Bucket> buckets, CancellationToken cancellationToken = default);

    void Remove(Bucket bucket);

    void Remove(IEnumerable<Bucket> buckets);

    Task RemoveAsync(Bucket bucket, CancellationToken cancellationToken = default);

    Task RemoveAsync(IEnumerable<Bucket> buckets, CancellationToken cancellationToken = default);

    void Update(Bucket bucket);

    void Update(IEnumerable<Bucket> buckets);

    Task UpdateAsync(Bucket bucket, CancellationToken cancellationToken = default);

    Task UpdateAsync(IEnumerable<Bucket> buckets, CancellationToken cancellationToken = default);
}
