namespace Scaffold.Domain.Exceptions
{
    public class BucketFullException : DomainException
    {
        public BucketFullException(string message)
            : base(message)
        {
        }

        public override string Detail => this.Message;

        public override string Title => "Bucket Full Exception";
    }
}
