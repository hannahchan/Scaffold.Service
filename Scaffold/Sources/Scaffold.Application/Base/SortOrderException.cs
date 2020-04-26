namespace Scaffold.Application.Base
{
    using System;
    using System.Runtime.Serialization;

    public abstract class SortOrderException : ApplicationException
    {
        protected SortOrderException(string message)
            : base(message)
        {
        }

        protected SortOrderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SortOrderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
