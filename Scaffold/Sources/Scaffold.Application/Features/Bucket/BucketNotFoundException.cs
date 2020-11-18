namespace Scaffold.Application.Features.Bucket
{
    using System;
    using System.Runtime.Serialization;
    using Scaffold.Application.Common.Exceptions;

    [Serializable]
    public class BucketNotFoundException : NotFoundException
    {
        public BucketNotFoundException(int bucketId)
            : base($"Bucket '{bucketId}' not found.")
        {
        }

        protected BucketNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
