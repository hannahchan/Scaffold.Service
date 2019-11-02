namespace Scaffold.Application.Features.Bucket
{
    using System;
    using System.Runtime.Serialization;
    using Scaffold.Application.Base;

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
