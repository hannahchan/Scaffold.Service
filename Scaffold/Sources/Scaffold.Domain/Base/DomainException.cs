namespace Scaffold.Domain.Base
{
    using System;
    using System.Runtime.Serialization;

    public abstract class DomainException : Exception
    {
        protected DomainException(string? message)
            : base(message)
        {
        }

        protected DomainException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected DomainException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
