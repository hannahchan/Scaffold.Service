namespace Scaffold.WebApi.Models.Item
{
    public class AddItemRequestBody
    {
        /// <summary>The name of the item being added.</summary>
        public string? Name { get; set; }

        /// <summary>The description of the item being added.</summary>
        public string? Description { get; set; }
    }
}
