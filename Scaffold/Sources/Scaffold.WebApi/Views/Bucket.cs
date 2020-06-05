namespace Scaffold.WebApi.Views
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class Bucket
    {
        [BindNever]
        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? Size { get; set; }
    }
}
