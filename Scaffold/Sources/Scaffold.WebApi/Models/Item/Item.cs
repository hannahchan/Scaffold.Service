namespace Scaffold.WebApi.Models.Item
{
    using System.ComponentModel.DataAnnotations;

    public class Item
    {
        /// <summary>The unique identifier of the item in the bucket.</summary>
        [Required]
        public int Id { get; set; }

        /// <summary>The name of the item.</summary>
        public string? Name { get; set; }

        /// <summary>A description of the item.</summary>
        public string? Description { get; set; }
    }
}
