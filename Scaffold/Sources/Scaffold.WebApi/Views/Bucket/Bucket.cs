namespace Scaffold.WebApi.Views.Bucket
{
    using System.ComponentModel.DataAnnotations;

    public class Bucket
    {
        [Required]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public int Size { get; set; }
    }
}
