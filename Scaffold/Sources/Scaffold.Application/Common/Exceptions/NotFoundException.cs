namespace Scaffold.Application.Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public abstract class NotFoundException : ApplicationException
    {
        protected NotFoundException(string message)
            : base(message)
        {
        }

        protected NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
