namespace Scaffold.Application.Exceptions
{
    public class PropertyNotComparableException : OrderingException
    {
        public PropertyNotComparableException(string propertyName)
            : base($"\"{propertyName}\" is not a comparable property.")
        {
        }

        public override string Detail => this.Message;

        public override string Title => "Property Not Comparable";
    }
}
