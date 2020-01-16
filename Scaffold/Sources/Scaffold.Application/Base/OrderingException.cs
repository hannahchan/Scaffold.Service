namespace Scaffold.Application.Base
{
    using System;
    using System.Runtime.Serialization;

    public abstract class OrderingException : ApplicationException
    {
        protected OrderingException(string message)
            : base(message)
        {
        }

        protected OrderingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected OrderingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
