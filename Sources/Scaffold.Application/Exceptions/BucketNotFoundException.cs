namespace Scaffold.Application.Exceptions
{
    using System;

    public class BucketNotFoundException : NotFoundException
    {
        public BucketNotFoundException(int bucketId)
            : base($"Bucket '{bucketId}' not found.")
        {
        }

        public override string Detail => this.Message;

        public override string Title => "Bucket Not Found";
    }
}
