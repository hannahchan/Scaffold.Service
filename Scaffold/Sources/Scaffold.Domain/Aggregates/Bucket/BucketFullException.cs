namespace Scaffold.Domain.Aggregates.Bucket
{
    using System;
    using System.Runtime.Serialization;
    using Scaffold.Domain.Base;

    [Serializable]
    public class BucketFullException : DomainException
    {
        public BucketFullException(string message)
            : base(message)
        {
        }

        protected BucketFullException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
