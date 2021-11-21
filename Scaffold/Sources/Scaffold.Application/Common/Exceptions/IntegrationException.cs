namespace Scaffold.Application.Common.Exceptions;

using System;
using System.Runtime.Serialization;

public abstract class IntegrationException : ApplicationException
{
    protected IntegrationException(string message, int status)
        : base(message)
    {
        this.Status = status;
    }

    protected IntegrationException(string message, int status, Exception innerException)
        : base(message, innerException)
    {
        this.Status = status;
    }

    protected IntegrationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        this.Status = info.GetInt32(nameof(this.Status));
    }

    public virtual int Status { get; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(this.Status), this.Status);
        base.GetObjectData(info, context);
    }
}
