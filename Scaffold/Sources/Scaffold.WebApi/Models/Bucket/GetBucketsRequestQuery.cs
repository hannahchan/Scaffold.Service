namespace Scaffold.WebApi.Models.Bucket;

public class GetBucketsRequestQuery
{
    /// <summary>The maximun number of buckets to return from the result set.</summary>
    public int? Limit { get; set; }

    /// <summary>The number of buckets to omit from the start of the result set.</summary>
    public int? Offset { get; set; }
}
