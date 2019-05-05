namespace Scaffold.Application.Exceptions
{
    public class DuplicateIdException : ApplicationException
    {
        public DuplicateIdException(string message)
            : base(message)
        {
        }

        public override string Detail => this.Message;

        public override string Title => "Duplicate Id.";
    }
}
