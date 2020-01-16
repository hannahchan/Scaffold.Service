namespace Scaffold.Domain.Aggregates.Bucket
{
    using System;
    using System.Runtime.Serialization;
    using Scaffold.Domain.Base;

    [Serializable]
    public class InvalidSizeException : DomainException
    {
        public InvalidSizeException(string message)
            : base(message)
        {
        }

        protected InvalidSizeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
