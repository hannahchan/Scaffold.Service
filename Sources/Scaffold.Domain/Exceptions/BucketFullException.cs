namespace Scaffold.Domain.Exceptions
{
    public class BucketFullException : DomainException
    {
        public BucketFullException(int bucketId)
            : base($"Bucket '{bucketId}' is full.")
        {
        }

        public override string Detail => this.Message;

        public override string Title => "Bucket Full Exception";
    }
}
