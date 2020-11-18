namespace Scaffold.Application.Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public abstract class ApplicationException : Exception
    {
        protected ApplicationException(string? message)
            : base(message)
        {
        }

        protected ApplicationException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected ApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
