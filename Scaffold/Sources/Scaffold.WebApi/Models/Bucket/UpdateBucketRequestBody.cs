namespace Scaffold.WebApi.Models.Bucket;

public class UpdateBucketRequestBody
{
    /// <summary>The new name of the bucket being updated.</summary>
    public string? Name { get; set; }

    /// <summary>The new description of the bucket being updated.</summary>
    public string? Description { get; set; }

    /// <summary>The new size of the bucket being update.</summary>
    public int? Size { get; set; }
}
