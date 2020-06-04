namespace Scaffold.WebApi.Views
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class Item
    {
        [BindNever]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}
