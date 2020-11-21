namespace Scaffold.WebApi.Views.Item
{
    using System.ComponentModel.DataAnnotations;

    public class Item
    {
        [Required]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}
