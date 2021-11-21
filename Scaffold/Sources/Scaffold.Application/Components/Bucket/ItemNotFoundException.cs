namespace Scaffold.Application.Components.Bucket;

using System;
using System.Runtime.Serialization;
using Scaffold.Application.Common.Exceptions;

[Serializable]
public class ItemNotFoundException : NotFoundException
{
    public ItemNotFoundException(int itemId)
        : base($"Item '{itemId}' not found.")
    {
    }

    protected ItemNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
