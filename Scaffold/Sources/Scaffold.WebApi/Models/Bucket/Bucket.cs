namespace Scaffold.WebApi.Models.Bucket;

using System.ComponentModel.DataAnnotations;

public class Bucket
{
    /// <summary>The unique identifier of the bucket.</summary>
    [Required]
    public int Id { get; set; }

    /// <summary>The name of the bucket.</summary>
    public string? Name { get; set; }

    /// <summary>A description of the bucket or the items in the bucket.</summary>
    public string? Description { get; set; }

    /// <summary>The maximum number of items the bucket can hold.</summary>
    [Required]
    public int Size { get; set; }
}
