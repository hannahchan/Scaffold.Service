namespace Scaffold.Domain.Exceptions
{
    public class InvalidSizeException : DomainException
    {
        public InvalidSizeException(string message)
            : base(message)
        {
        }

        public override string Detail => this.Message;

        public override string Title => "Invalid Size";
    }
}
