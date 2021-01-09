namespace Scaffold.WebApi.Models.Bucket
{
    public class AddBucketRequestBody
    {
        /// <summary>The name of the bucket being added.</summary>
        public string? Name { get; set; }

        /// <summary>The description of the bucket being added.</summary>
        public string? Description { get; set; }

        /// <summary>The size of the bucket being added.</summary>
        public int? Size { get; set; }
    }
}
