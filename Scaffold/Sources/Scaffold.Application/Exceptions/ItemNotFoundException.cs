namespace Scaffold.Application.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ItemNotFoundException : NotFoundException
    {
        public ItemNotFoundException(int itemId)
            : base("Item Not Found", $"Item '{itemId}' not found.")
        {
        }

        protected ItemNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
