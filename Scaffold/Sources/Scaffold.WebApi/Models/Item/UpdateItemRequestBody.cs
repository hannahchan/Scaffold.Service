namespace Scaffold.WebApi.Models.Item;

public class UpdateItemRequestBody
{
    /// <summary>The new name of the item being updated.</summary>
    public string? Name { get; set; }

    /// <summary>The new description of the item being updated.</summary>
    public string? Description { get; set; }
}
