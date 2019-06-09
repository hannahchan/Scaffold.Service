namespace Scaffold.Domain.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class BucketFullException : DomainException
    {
        public BucketFullException(string message)
            : base("Bucket Full", message)
        {
        }

        protected BucketFullException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
