namespace Scaffold.Application.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class BucketNotFoundException : NotFoundException
    {
        public BucketNotFoundException(int bucketId)
            : base("Bucket Not Found", $"Bucket '{bucketId}' not found.")
        {
        }

        protected BucketNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
