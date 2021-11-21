namespace Scaffold.Application.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Scaffold.Application.Common.Models;
using Scaffold.Domain.Aggregates.Bucket;

public interface IBucketReadRepository
{
    Bucket? Get(int id);

    List<Bucket> Get(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null, SortOrder<Bucket>? sortOrder = null);

    Task<Bucket?> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<List<Bucket>> GetAsync(Expression<Func<Bucket, bool>> predicate, int? limit = null, int? offset = null, SortOrder<Bucket>? sortOrder = null, CancellationToken cancellationToken = default);
}
