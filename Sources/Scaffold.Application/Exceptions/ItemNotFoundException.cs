namespace Scaffold.Application.Exceptions
{
    using System;

    public class ItemNotFoundException : NotFoundException
    {
        public ItemNotFoundException(int itemId)
            : base($"Item '{itemId}' not found.")
        {
        }

        public override string Detail => this.Message;

        public override string Title => "Item Not Found";
    }
}
